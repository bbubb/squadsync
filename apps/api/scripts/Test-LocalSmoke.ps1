#Requires -Version 5.1
[CmdletBinding()]
param(
    [ValidateRange(1024, 65535)][int]$ApiPort = 5050,
    [ValidateRange(10, 300)][int]$TimeoutSeconds = 90
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'
$api = $null
$apiStarted = $false
$containerAttempted = $false
$failed = $false
$token = [Guid]::NewGuid().ToString('N')
$containerName = "squadsync-smoke-$token"
$ownerLabel = 'com.squadsync.local-smoke'
$apiRoot = Split-Path $PSScriptRoot -Parent
$repoRoot = Split-Path (Split-Path $apiRoot -Parent) -Parent
$composeFile = Join-Path $repoRoot 'infra/docker/compose.yaml'
$envFile = Join-Path $repoRoot 'infra/docker/.env'
$compose = @('compose', '--project-name', 'squadsync', '--file', $composeFile, '--env-file', $envFile)

function Stop-Smoke {
    param([string]$Message)
    $exception = New-Object System.Exception $Message
    $exception.Data['LocalSmokeDiagnostic'] = $true
    throw $exception
}

function Get-SafeDiagnostic {
    param($Exception)
    if ($Exception.Data['LocalSmokeDiagnostic']) { return $Exception.Message }
    return "Unexpected smoke-script/configuration failure ($($Exception.GetType().Name)); details suppressed to protect credentials."
}

function New-ChildProcess {
    param([string]$File, [string[]]$Arguments)
    $start = New-Object System.Diagnostics.ProcessStartInfo
    $start.FileName = $File
    # All arguments are repository paths or script-controlled values, never credentials.
    $start.Arguments = ($Arguments | ForEach-Object { '"' + $_ + '"' }) -join ' '
    $start.WorkingDirectory = $apiRoot
    $start.UseShellExecute = $false
    $start.CreateNoWindow = $true
    $start.RedirectStandardOutput = $true
    $start.RedirectStandardError = $true
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $start
    return $process
}

function Invoke-LocalCommand {
    param([string]$File, [string[]]$Arguments, [string]$Failure)
    $process = New-ChildProcess $File $Arguments
    try {
        try { [void]$process.Start() }
        catch { Stop-Smoke "Could not launch $File from $apiRoot. Check executable access and working directory (error type $($_.Exception.InnerException.GetType().Name))." }
        $stdout = $process.StandardOutput.ReadToEndAsync()
        $stderr = $process.StandardError.ReadToEndAsync()
        if (-not $process.WaitForExit($TimeoutSeconds * 1000)) {
            $process.Kill()
            $process.WaitForExit()
            Stop-Smoke "$Failure (command timed out after $TimeoutSeconds seconds)."
        }
        $output = $stdout.GetAwaiter().GetResult()
        $errors = $stderr.GetAwaiter().GetResult()
        if ($process.ExitCode -ne 0) {
            # Never echo native output: Compose config and application errors can contain secrets.
            if ($File -eq $script:docker -and $errors -match 'Access is denied|permission denied|docker_engine|dockerDesktopLinuxEngine|cannot connect|Is the docker daemon running') {
                Stop-Smoke 'Docker/Desktop access blocker: check Docker Desktop is running in Linux-container mode and this Windows account can read Docker configuration and access its named pipe.'
            }
            Stop-Smoke "$Failure (exit $($process.ExitCode); native output suppressed to protect local credentials)."
        }
        return $output.Trim()
    }
    finally { $process.Dispose() }
}

function Invoke-Docker {
    param([string[]]$Arguments, [string]$Failure = 'Docker command failed')
    Invoke-LocalCommand $script:docker $Arguments $Failure
}

function Assert-PortFree {
    param([int]$Port)
    if ([System.Net.NetworkInformation.IPGlobalProperties]::GetIPGlobalProperties().GetActiveTcpListeners().Port -contains $Port) {
        Stop-Smoke "Port $Port is already occupied. Stop: no existing process will be changed."
    }
    $listener = New-Object System.Net.Sockets.TcpListener ([System.Net.IPAddress]::Loopback), $Port
    try { $listener.Start() }
    catch { Stop-Smoke "Port $Port cannot be reserved. Stop: ownership is ambiguous." }
    finally { $listener.Stop() }
}

function Get-OwnedContainer {
    $ids = Invoke-Docker @('ps', '-aq', '--filter', "name=^/$containerName$")
    if (-not $ids) { return $null }
    $details = Invoke-Docker @('inspect', $ids) | ConvertFrom-Json
    if (@($details).Count -ne 1) {
        Stop-Smoke 'Container ownership is ambiguous; refusing to stop, restart or remove it.'
    }
    $label = $details[0].Config.Labels.PSObject.Properties[$ownerLabel]
    if ($null -eq $label -or $label.Value -ne $token) {
        Stop-Smoke 'Container ownership is ambiguous; refusing to stop, restart or remove it.'
    }
    return $details[0]
}

function Wait-Postgres {
    $timer = [Diagnostics.Stopwatch]::StartNew()
    do {
        $owned = Get-OwnedContainer
        if ($null -eq $owned) { Stop-Smoke 'The owned PostgreSQL container disappeared.' }
        if ($owned.State.Running -and $owned.State.Health.Status -eq 'healthy') { return }
        Start-Sleep -Seconds 1
    } while ($timer.Elapsed.TotalSeconds -lt $TimeoutSeconds)
    Stop-Smoke "PostgreSQL did not become healthy within $TimeoutSeconds seconds. Check local image, port and volume compatibility; no data was reset."
}

function Wait-Health {
    param([int]$ReadyStatus, [string]$State)
    $timer = [Diagnostics.Stopwatch]::StartNew()
    $live = 0
    $ready = 0
    do {
        if ($api.HasExited) { Stop-Smoke "API exited before $State (exit $($api.ExitCode)); check local API configuration/build. Logs are suppressed to protect credentials." }
        $statuses = foreach ($path in @('/health', '/health/ready')) {
            try {
                $request = [System.Net.HttpWebRequest]::Create("http://127.0.0.1:$ApiPort$path")
                $request.Proxy = $null
                $request.Timeout = 3000
                $response = $request.GetResponse()
                try { [int]$response.StatusCode } finally { $response.Close() }
            }
            catch [System.Net.WebException] {
                if ($null -ne $_.Exception.Response) {
                    try { [int]$_.Exception.Response.StatusCode } finally { $_.Exception.Response.Close() }
                }
                else { 0 }
            }
        }
        $live, $ready = $statuses
        if ($live -eq 200 -and $ready -eq $ReadyStatus) {
            try { $listeners = @(Get-NetTCPConnection -LocalPort $ApiPort -State Listen -ErrorAction Stop) }
            catch { Stop-Smoke 'Cannot inspect API port ownership. Run on the approved Windows host with TCP connection inspection access.' }
            if ($listeners.Count -ne 1 -or $listeners[0].OwningProcess -ne $api.Id) {
                Stop-Smoke 'API port ownership is ambiguous; refusing to accept health responses from another process.'
            }
            Write-Host "$State : /health=200 /health/ready=$ReadyStatus"
            return
        }
        Start-Sleep -Seconds 1
    } while ($timer.Elapsed.TotalSeconds -lt $TimeoutSeconds)
    Stop-Smoke "$State failed: expected 200/$ReadyStatus, last observed $live/$ready. Check API/database settings; this is an endpoint validation failure."
}

try {
    try {
        $docker = (Get-Command docker -CommandType Application | Select-Object -First 1).Source
        $dotnet = (Get-Command dotnet -CommandType Application | Select-Object -First 1).Source
        [void](Get-Command Get-NetTCPConnection)
    }
    catch { Stop-Smoke 'Missing prerequisites: use Windows PowerShell with Docker CLI/Compose, .NET 10 and Get-NetTCPConnection available.' }
    if (-not (Test-Path -LiteralPath $envFile)) { Stop-Smoke 'Missing ignored infra/docker/.env. Copy .env.example and configure local PostgreSQL first.' }
    Assert-PortFree $ApiPort
    [void](Invoke-Docker @('info', '--format', '{{.ServerVersion}}'))
    [void](Invoke-Docker @('compose', 'version'))
    $existing = Invoke-Docker @('ps', '-aq', '--filter', 'label=com.docker.compose.project=squadsync', '--filter', 'label=com.docker.compose.service=postgres')
    if ($existing) { Stop-Smoke 'Existing Compose PostgreSQL container found (including stopped/one-off containers). Stop: leave it untouched and run only when the local Compose service has been deliberately removed by its owner.' }

    # Let Compose resolve .env quoting/interpolation and environment precedence; never print config.
    $config = Invoke-Docker ($compose + @('config', '--format', 'json')) 'Compose configuration failed; check ignored infra/docker/.env' | ConvertFrom-Json
    $postgres = $config.services.postgres
    $databasePort = [int]$postgres.ports[0].published
    if ($ApiPort -eq $databasePort) { Stop-Smoke 'API and PostgreSQL ports must differ.' }
    Assert-PortFree $databasePort
    $volumeName = $config.volumes.postgres_data.name
    $volumeUsers = Invoke-Docker @('ps', '-aq', '--filter', "volume=$volumeName")
    if ($volumeUsers) { Stop-Smoke 'An existing container uses the PostgreSQL data volume. Stop: exclusive database ownership cannot be established.' }

    Write-Host 'Building API (no database tests or migrations)...'
    [void](Invoke-LocalCommand $dotnet @('build', 'src/SquadSync.Api', '--nologo') 'API build/restore failed; run dotnet build from apps/api for diagnostics')
    Assert-PortFree $ApiPort
    Assert-PortFree $databasePort
    # Recheck after the build so another developer's newly started service is not disturbed.
    $existing = Invoke-Docker @('ps', '-aq', '--filter', 'label=com.docker.compose.project=squadsync', '--filter', 'label=com.docker.compose.service=postgres')
    $volumeUsers = Invoke-Docker @('ps', '-aq', '--filter', "volume=$volumeName")
    if ($existing -or $volumeUsers) { Stop-Smoke 'PostgreSQL ownership changed during preflight. Stop: existing resources were left untouched.' }
    # A unique name plus per-run label lets finally recover even a partially failed Compose run.
    $containerAttempted = $true
    [void](Invoke-Docker ($compose + @('run', '--detach', '--no-deps', '--service-ports', '--name', $containerName, '--label', "$ownerLabel=$token", 'postgres')) 'Compose PostgreSQL startup failed; check image availability and local Docker settings')
    Wait-Postgres

    # DbConnectionStringBuilder safely quotes passwords containing connection-string delimiters.
    $connection = New-Object System.Data.Common.DbConnectionStringBuilder
    $connection['Host'] = '127.0.0.1'
    $connection['Port'] = $databasePort
    $connection['Database'] = $postgres.environment.POSTGRES_DB
    $connection['Username'] = $postgres.environment.POSTGRES_USER
    $connection['Password'] = $postgres.environment.POSTGRES_PASSWORD
    $dll = Join-Path $apiRoot 'src/SquadSync.Api/bin/Debug/net10.0/SquadSync.Api.dll'
    Assert-PortFree $ApiPort
    $api = New-ChildProcess $dotnet @($dll, '--urls', "http://127.0.0.1:$ApiPort", '--environment', 'Production', '--seed-demo=false')
    $api.StartInfo.WorkingDirectory = Join-Path $apiRoot 'src/SquadSync.Api'
    $api.StartInfo.EnvironmentVariables['ConnectionStrings__SquadSync'] = $connection.ConnectionString
    # Production exposes only health endpoints. Disable logging so secrets cannot enter captured logs.
    $api.StartInfo.EnvironmentVariables['Serilog__MinimumLevel__Default'] = 'Fatal'
    [void]$api.Start()
    $apiStarted = $true
    $apiOutput = $api.StandardOutput.ReadToEndAsync()
    $apiErrors = $api.StandardError.ReadToEndAsync()
    Wait-Health 200 'PostgreSQL running'
    $owned = Get-OwnedContainer
    if ($null -eq $owned) { Stop-Smoke 'Owned PostgreSQL container missing before stop.' }
    [void](Invoke-Docker @('stop', '--time', '10', $owned.Id))
    Wait-Health 503 'PostgreSQL stopped'
    $owned = Get-OwnedContainer
    if ($null -eq $owned) { Stop-Smoke 'Owned PostgreSQL container missing before restart.' }
    [void](Invoke-Docker @('start', $owned.Id))
    Wait-Postgres
    Wait-Health 200 'PostgreSQL restarted'
}
catch {
    $failed = $true
    [Console]::Error.WriteLine("Local smoke FAILED: $(Get-SafeDiagnostic $_.Exception)")
    if (-not $_.Exception.Data['LocalSmokeDiagnostic']) { [Console]::Error.WriteLine($_.ScriptStackTrace) }
}
finally {
    if ($null -ne $api) {
        try {
            if ($apiStarted -and -not $api.HasExited) { $api.Kill(); $api.WaitForExit() }
        }
        catch { $failed = $true; [Console]::Error.WriteLine('Cleanup FAILED: could not stop the owned API process.') }
        finally { $api.Dispose() }
    }
    if ($containerAttempted) {
        try {
            $owned = Get-OwnedContainer
            if ($null -ne $owned) { [void](Invoke-Docker @('rm', '--force', $owned.Id) 'Owned PostgreSQL cleanup failed') }
            Write-Host 'Owned PostgreSQL container removed; named data volume and shared Compose network preserved.'
        }
        catch { $failed = $true; [Console]::Error.WriteLine("Cleanup FAILED for $containerName : $(Get-SafeDiagnostic $_.Exception)") }
    }
}
if ($failed) { exit 1 }
Write-Host 'Local smoke PASSED; owned resources cleaned up.'
exit 0
