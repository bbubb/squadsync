using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace SquadSync.IntegrationTests;

// Registered only by the test host; this assembly is never referenced by the shipped API.
[ApiController]
[Route("api/management-probe")]
public sealed class ManagementProbeController : ControllerBase
{
    [HttpPost]
    public IActionResult Create(ProbeRequest request) => Ok(new { request.Name });

    [HttpGet("missing")]
    public IActionResult Missing() => NotFound();

    [HttpGet("throw")]
    public IActionResult Throw() => throw new InvalidOperationException(
        "Host=private-db;Username=admin;Password=secret-probe-password");

    [HttpGet("cancel")]
    public IActionResult Cancel()
    {
        HttpContext.RequestAborted = new CancellationToken(canceled: true);
        throw new OperationCanceledException(HttpContext.RequestAborted);
    }
}

public sealed record ProbeRequest([Required] string? Name);
