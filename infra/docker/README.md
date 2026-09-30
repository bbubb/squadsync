# Local Docker Infrastructure

This directory provides PostgreSQL for local development only. The database runs separately from the API and does not define production or cloud infrastructure.

## Start PostgreSQL

From the repository root, copy the example settings to the ignored local environment file:

```powershell
Copy-Item infra/docker/.env.example infra/docker/.env
```

The committed `.env.example` contains local development defaults only. `.env` is ignored by Git; keep machine-specific credentials there and do not commit them.

Start PostgreSQL from `infra/docker/` so Compose loads that local `.env` file:

```powershell
Set-Location infra/docker
docker compose config
docker compose up -d
docker compose ps
```

Follow startup or troubleshooting output with:

```powershell
docker compose logs -f postgres
```

The service uses the Docker Official Image `postgres:18-alpine`, publishes port 5432 on loopback only, and reports healthy after `pg_isready` confirms PostgreSQL accepts connections. Set `POSTGRES_PORT` in `.env` to use another available local port.

Connect from the host using the configured `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_DB` values. The database is available at `localhost` on `POSTGRES_PORT` (5432 by default). Other Compose services can reach it at `postgres:5432` on the Compose network.

With the example settings, the API connection string is `Host=127.0.0.1;Port=5432;Database=squadsync;Username=squadsync;Password=local_dev_password`. Use the matching port and credentials if you changed `.env`.

## Stop PostgreSQL

From `infra/docker/`, stop the container while keeping its database volume:

```powershell
docker compose down
```

The named `postgres_data` volume persists across container recreation. `docker compose down` leaves it intact. Removing the volume requires the explicit destructive command `docker compose down -v`; this deletes the local database data.
