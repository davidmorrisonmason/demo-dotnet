# demo
Demo code base for example architectures.

## Development setup

Install the .NET 10 SDK, PowerShell 7, and Docker Desktop (Windows) or Docker Engine
with Compose v2.20.0 or newer (Linux). Start Docker. On Windows, select Linux
containers; the SQL Server image requires an x86-64 Linux engine. Allow at least
4 GB of memory for Docker. SQL Server installation and SQL management tools are
not required.

From the repository root:

```powershell
pwsh ./scripts/dev-setup.ps1
pwsh ./scripts/dev-run-api.ps1
```

Setup generates credentials in the Git-ignored `.dev/sqlserver.env`, starts the
pinned SQL Server 2022 Developer container, waits for a successful SQL query,
restores the local EF tool, applies migrations, and seeds an empty database.
The API runs at http://localhost:5272 (Swagger at `/swagger`). Demo API keys are
`test-api-key` and `test-api-key-2`; send one in the `x-api-key` header.

Setup can be rerun. It keeps the same password and persistent database volume,
and skips seeding if any application data already exists, including soft-deleted
rows. Seeding an empty database is atomic; a failure rolls back the entire seed.
Concurrent SQL Server seed runs are serialized. It never truncates existing data
or repairs a partly populated database by overwriting it.

The default SQL port is `127.0.0.1:14333`, database `Demo`. If that port is occupied,
use `pwsh ./scripts/dev-setup.ps1 -SqlPort 14334` on first setup. Each checkout has
its own Compose project and volume; choose different ports for simultaneous
checkouts. To change an existing checkout's port, stop the database, edit
`DEMO_SQL_PORT` in `.dev/sqlserver.env`, then rerun setup. Retain this file with
the volume: changing or losing its password does not change SQL Server's saved
password. Do not commit or share the file or print Compose's expanded configuration.

The scripts explicitly select their own localhost connection and Development
environment, then restore the caller's environment. They do not use an inherited
production connection string. `dev-run-api.ps1` supplies the generated connection
to the API; plain `dotnet run` continues to use the project's configured settings.
The container port is bound to loopback. Certificate trust is enabled only for
the scripts' local development connection. Developer edition and `sa` credentials
are for local development only.

Stop the database without deleting data:

```powershell
pwsh ./scripts/dev-stop.ps1
```

To permanently delete **this checkout's Docker development database**, stop the
API and explicitly run:

```powershell
pwsh ./scripts/dev-reset.ps1 -DeleteDevelopmentData
pwsh ./scripts/dev-setup.ps1
```

Reset removes the development container and its volume, retaining local credentials
for the next setup. It does not connect to or reset SQL Express, LocalDB, test
databases, or an externally configured SQL Server.

If setup fails, fix the reported prerequisite, port or database issue and rerun.
The first run downloads a large SQL Server image and NuGet packages. A failed
readiness check leaves the container available for inspection; rerun setup after
fixing Docker's memory allocation or configuration. Stop the API before reset.

## Tests

```powershell
dotnet test
```

Tests currently retain their SQLite fixtures; disposable SQL Server test containers
are the next migration step. Docker is not yet required to run these tests.

Reference: [SQL Server containers](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker)
and [Compose environment files](https://docs.docker.com/compose/how-tos/environment-variables/variable-interpolation/).
