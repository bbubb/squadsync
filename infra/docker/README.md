# Local Docker Infrastructure

This directory provides PostgreSQL for local development only. The database runs separately from the API and does not define production or cloud infrastructure.

## Start PostgreSQL

From the repository root, copy the example settings to the ignored local environment file:

```powershell
Copy-Item infra/docker/.env.example infra/docker/.env
```

The example password is for local development only. Change it in `infra/docker/.env` if desired; `.env` files are ignored by Git.

Start the service from this directory so Compose loads the local `.env` file:

```powershell
Set-Location infra/docker
docker compose config
docker compose up -d
docker compose ps
```

The service uses the Docker Official Image `postgres:18-alpine`, publishes port 5432 on loopback only, and reports healthy after `pg_isready` confirms PostgreSQL accepts connections. Set `POSTGRES_PORT` in `.env` to use another available local port.

Connect from the host using the configured `POSTGRES_USER`, `POSTGRES_PASSWORD`, and `POSTGRES_DB` values. The database is available at `localhost` on `POSTGRES_PORT` (5432 by default). Other Compose services can reach it at `postgres:5432` on the Compose network.

## Stop PostgreSQL

From `infra/docker/`:

```powershell
docker compose down
```

The named `postgres_data` volume persists across container recreation. `docker compose down` leaves it intact. Removing the volume requires the explicit destructive command `docker compose down -v`; this deletes the local database data.
