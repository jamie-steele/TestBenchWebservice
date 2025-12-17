# TestBenchWebService (Database Test Bench API)

ASP.NET Core (.NET 8) web API that exposes simple **test-bench endpoints** for multiple database providers so you can validate connectivity and run a small “known query” set per DB.

It’s designed to be **plug-and-play**:
- **No secrets in repo**: `appsettings*.json` contains blank placeholders.
- **Environment variables override config** (recommended for deployments).
- Endpoints are **self-healing**: each request reloads config/env vars, recreates the connection, and **retries once** on failure.

## Supported databases
- PostgreSQL (EF Core)
- MySQL / MariaDB (EF Core)
- SQL Server (EF Core)
- Oracle (EF Core)
- Snowflake (Snowflake driver + Dapper)

## Running locally

### Build

```bash
dotnet build TestBenchWebService.sln -c Release
```

### Run

```bash
dotnet run --project TestBenchWebService.csproj
```

### Swagger
- Swagger UI is served at the site root: **`/`**
- OpenAPI JSON: **`/swagger/v1/swagger.json`**

## Configuration

### Connection strings (preferred: environment variables)
Set these to enable each provider:

- `ConnectionStrings__PostgreSQL`
- `ConnectionStrings__MySQL`
- `ConnectionStrings__SqlServer`
- `ConnectionStrings__Oracle`
- `ConnectionStrings__Snowflake`

If a connection string is blank/missing, the matching endpoints return **400 Not Configured**.

### Example env var snippets

```bash
export ConnectionStrings__PostgreSQL="Host=localhost;Port=5432;Database=blogdb;Username=bloguser;Password=bloguser"
export ConnectionStrings__MySQL="Server=localhost;Port=3306;Database=blogdb;Uid=bloguser;Pwd=bloguser;"
export ConnectionStrings__SqlServer="Server=localhost,1433;Database=blogdb;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=true;"
export ConnectionStrings__Oracle="Data Source=localhost:1521/XE;User Id=bloguser;Password=bloguser;"
export ConnectionStrings__Snowflake="account=...;user=...;password=...;db=...;schema=...;warehouse=...;role=..."
```

## API endpoints
Each DB exposes:
- `GET /api/<DbName>/test-connection`
- `GET /api/<DbName>`
- `GET /api/<DbName>/{title}`

Where `<DbName>` is one of:
- `Postgres`
- `MySql`
- `SqlServer`
- `Oracle`
- `Snowflake`

## Notes
- The service may seed a small set of sample rows on demand (used by the list/query endpoints).
- For containerized deployments, configure connection strings via env vars (recommended).
