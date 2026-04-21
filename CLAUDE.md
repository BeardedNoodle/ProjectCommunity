# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Running and building

Everything is orchestrated by Aspire. Do not start pieces individually unless debugging one in isolation.

- **Run the whole stack** (Postgres, Redis, Server, frontend):
  `dotnet run --project src/PortfolioProject.AppHost`
  The Aspire dashboard prints a URL with the server and frontend endpoints.
- **Build the backend:** `dotnet build PortfolioProject.slnx`
- **Frontend standalone dev:** `cd frontend && npm run dev` (requires `SERVER_HTTPS` or `SERVER_HTTP` env var pointing at a running Server for the `/api` proxy)
- **Frontend build/lint:** `cd frontend && npm run build` / `npm run lint`

### EF Core migrations

DbContext lives in Infrastructure but the startup project is Server:

```
dotnet ef migrations add <Name> \
  --project src/PortfolioProject.Infrastructure \
  --startup-project src/PortfolioProject.Server
```

### Tests

No test project exists yet. If adding one, place it at `tests/<Name>.Tests/` and add it to `PortfolioProject.slnx`.

## Architecture

Clean Architecture in `src/`, React+Vite frontend in `frontend/`, orchestrated by Aspire AppHost.

### Layer dependencies (strict, one-way)

`Domain` ← `Application` ← `Infrastructure` ← `Server`

- **Domain** — POCOs only, no dependencies. `Common/BaseEntity.cs` provides `Id`, audit fields (`CreatedAt`/`CreatedBy`, `UpdatedAt`/`UpdatedBy`) and soft-delete fields (`IsDeleted`, `DeletedAt`/`DeletedBy`) — every entity inherits these. Current entities under `Entities/`: `User`, `Portfolio`, `Asset`, `Transaction`, `Price`. Enums under `Enums/`: `AssetClass`, `TransactionType`.
- **Application** — MediatR handlers + FluentValidation validators. Both auto-registered from the assembly in `DependencyInjection.AddApplicationServices`. Add new features as `IRequest`/`IRequestHandler` pairs; no manual registration needed. `Common/Interfaces/ICurrentUser.cs` abstracts the calling user — handlers/interceptors should depend on this, not on `HttpContext`.
- **Infrastructure** — Single project (do not split). EF Core + Npgsql via Aspire's `AddNpgsqlDbContext<ApplicationDbContext>("portfoliodb")`. `ApplicationDbContext.OnModelCreating` calls `ApplyConfigurationsFromAssembly`, so drop `IEntityTypeConfiguration<T>` classes anywhere in the project (convention: `Persistence/Configurations/`) and they are picked up automatically. Shared `BaseEntityConfiguration<T>` sets the PK and applies the soft-delete query filter — inherit from it in each entity config. `Persistence/Interceptors/AuditSaveChangesInterceptor.cs` auto-populates audit fields and converts deletes into soft deletes (see Persistence patterns below). `Identity/CurrentUser.cs` is the current stub implementation of `ICurrentUser`. Background jobs (planned Discord/Telegram bots, scheduled reports) belong here as `IHostedService` implementations registered in `AddInfrastructureServices` — **not** a separate Worker project.
- **Server** — Minimal APIs only (no controllers). All endpoints go under the `/api` route group defined in `Program.cs`. Group endpoints by feature via extension methods on `RouteGroupBuilder`. Redis output caching is wired through `AddRedisClientBuilder("cache").WithOutputCache()`; apply with `.CacheOutput()` on endpoints that need it.
- **ServiceDefaults** — Standard Aspire package: service discovery, HTTP resilience, health checks (`/health`, `/alive`), OpenTelemetry. Referenced by Server via `AddServiceDefaults()` and `MapDefaultEndpoints()`. Leave alone unless changing cross-cutting telemetry.

### How the pieces talk at runtime

`AppHost.cs` declares the topology:

- `cache` (Redis) and `postgres` (with database `portfoliodb`) come up first.
- `server` waits for both, exposes `/health`, and has external HTTP endpoints.
- `webfrontend` (Vite) waits for `server`. In dev, Vite proxies `/api` to the server using `SERVER_HTTPS` / `SERVER_HTTP` env vars that Aspire injects.
- On publish, `server.PublishWithContainerFiles(webfrontend, "wwwroot")` bakes the built frontend into the server container; `UseFileServer()` serves it. There is one deployable: the Server image.

### Conventions

- .NET 10, nullable + implicit usings enabled globally via `Directory.Build.props`.
- Solution file is `PortfolioProject.slnx` (new XML format) — use it, not a `.sln`.
- Side-project scope: prefer the simplest thing that works. Don't split Infrastructure, don't add a Worker project, don't introduce controllers.

### Persistence patterns

- **Audit fields** are populated automatically by `AuditSaveChangesInterceptor` on every `SaveChanges`. Do not set `CreatedAt`/`UpdatedAt`/etc. manually — the interceptor handles it using `ICurrentUser.UserId`.
- **Soft delete** is the default. `context.Remove(entity); SaveChanges();` is intercepted and rewritten as an UPDATE setting `IsDeleted = true`. Queries automatically filter out soft-deleted rows via a global `HasQueryFilter(e => !e.IsDeleted)` applied by `BaseEntityConfiguration<T>`.
  - To read soft-deleted rows: `.IgnoreQueryFilters()` on the query.
  - To truly delete a row (rare — e.g. GDPR erasure, purging old `Price` rows): bypass EF with `context.Database.ExecuteSqlAsync(...)`.
- **Decimal precision**: monetary fields (`Amount`, `Fees`) use `decimal(18,4)`; share/price/ratio fields (`Quantity`, `Price`, `SplitRatio`, OHLC) use `decimal(18,8)` to accommodate crypto. Configure with `HasPrecision` in entity configurations.
- **Foreign keys default to `DeleteBehavior.Restrict`** — cascade deletes would interact badly with soft delete and audit history.

### Authentication

Auth is planned via **Keycloak** (not yet wired — `CurrentUser` in Infrastructure currently returns `null`). Decisions already made:

- The `User` entity is a local profile mirror. `User.ExternalId` stores the Keycloak `sub` claim.
- No password, role, or email-confirmation fields on `User` — those belong to Keycloak. Roles come from JWT claims at request time.
- When wiring Keycloak, replace `Infrastructure/Identity/CurrentUser.cs` with an implementation that reads `HttpContext.User` and maps the `sub` claim to the local `User.Id` (create the local row on first login).

### Alpaca integration

Do **not** bulk-import Alpaca's full asset catalog. `Asset` rows are lazy-populated on demand via a get-or-create flow in the Application layer: when a user first references a new symbol (transaction, watchlist, etc.), fetch details from Alpaca and persist one `Asset` row. `Asset.ExternalId` stores the Alpaca UUID for later refresh.

For market data:

- `Price` entity holds **daily close bars** for historical charts and reports. A background `IHostedService` should pull these after market close for existing `Asset` rows only.
- **Live/current prices** should go through Redis-cached Alpaca calls, not the `Price` table.
- **Intra-day bars** are not stored — fetch on demand from Alpaca if a feature needs them.
