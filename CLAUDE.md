# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project state

A Meeting Room Booking System REST API (ASP.NET Core, .NET 10, controller-based, SQL Server via EF
Core Code First). User auth (register/login/logout) and the start of the Meeting Rooms module are
implemented; Booking creation/availability/concurrency/recurring bookings are not yet built. There is
no MVC UI yet — everything is exercised through the REST API and Swagger. There is no test project
(tests are written ad hoc to verify a specific change, then deleted rather than kept — see "Testing").

## Solution layout

- `HotelBooking.slnx` — solution file (XML-based slnx format).
- `HotelBooking/HotelBooking.csproj` — the Web API project (`Microsoft.NET.Sdk.Web`, `net10.0`,
  nullable + implicit usings enabled, `GenerateDocumentationFile` on for Swagger XML docs).
- `HotelBooking/.env` — local-only `DB_CONNECTION_STRING`, loaded via `DotNetEnv.Env.Load()` at the
  top of `Program.cs`. Gitignored. Points at `(localdb)\mssqllocaldb` by default.

## Common commands

Run from `HotelBooking/HotelBooking` unless noted.

```bash
# restore / build
dotnet restore
dotnet build

# run (profiles in launchSettings.json: http on :5062, https on :7175/:5062)
dotnet run --urls https://localhost:7175
```

EF Core migrations:

```bash
dotnet ef migrations add <Name>
dotnet ef database update
```

No test project currently exists. When verifying a change, write a throwaway test/smoke-check, confirm
it, then delete it — tests are not kept around unless explicitly asked to keep them.

## Architecture

Single project, folder-based layering (no separate class libraries):

```
Controllers/  →  Services/  →  Repositories/  →  Data/AppDbContext
```

Controllers only call services and translate the result to an HTTP response; they never touch
`AppDbContext` or a repository directly, and never contain business logic.

**Entities** (`Models/Entities/`) all inherit `Models/BaseTypes/BaseEntity.cs`
(`CreatedAt`/`UpdatedAt`/`DeletedAt`). `DeletedAt` is a soft-delete marker — honored explicitly per
query (no global EF query filter exists), so any new repository method that reads by id/unique key
must add `DeletedAt == null` itself, matching `UserRepository.GetByEmail` /
`MeetingRoomRepository.GetByNameAndLocation`.

**Repositories** (`Repositories/`): a generic `Repository<T> : IRepository<T>` (`BaseRepository/`,
`Interfaces/`) provides `GetByIdAsync`, `GetAllAsync`, `AddAsync`, `SaveChangesAsync` — constrained to
`where T : BaseEntity`. Entity-specific repositories (`UserRepository`, `MeetingRoomRepository`) extend
it for lookups the generic base can't express (`GetByEmail`, `GetByNameAndLocation`).
`GetByIdAsync`/`GetAllAsync` deliberately do **not** use `FindAsync`/plain `Set<T>` — EF Core's
`FindAsync` doesn't reliably apply extra `Where` filtering the way a real query does, so both use a
`Where(...).FirstOrDefaultAsync/ToListAsync` query instead (matching by `EF.Property<int>(e, "Id")`
since `BaseEntity` doesn't declare `Id` itself — every entity's key is named `Id` by convention).

**Naming convention**: the `Async` suffix is used *only* on the generic `Repository<T>` base methods.
Entity-specific repository methods (`GetByEmail`, `GetByNameAndLocation`) and all service methods
(`Register`, `Login`, `Create`, `GetAll`, `GetById`) omit it, even though they're all `async`.

**Services** (`Services/`) contain all business logic and validation orchestration. They return
`ResponseWrapper`/`ResponseWrapper<T>` (`Models/Responses/ResponseWrapper.cs`) for *expected* business
outcomes — duplicate email/room name, not found, invalid credentials — rather than throwing. Controllers
just do `return StatusCode(response.StatusCode, response);`. Exceptions are reserved for genuinely
unexpected conditions and are only ever caught in `ExceptionHandlingMiddleware` — no other file in the
codebase has a try/catch.

Dedicated response DTOs (e.g. `GetMeetingRoomsResponse`) are only created when a `ResponseWrapper<T>`'s
`Data` would otherwise carry more than one field; a single scalar (e.g. a new row's `int Id`) stays as
`ResponseWrapper<int>` rather than being wrapped in a one-field record.

`ResponseWrapper<T>`'s `Data` is `T?`. For an unconstrained generic parameter, `T?` only means nullable
when `T` is a reference type — for `T = int` the property is plain `int`, so use `default`, not `null`,
when there's no data to return (works for both value and reference types uniformly).

**Validation**: FluentValidation validators (`Validators/`) are registered as
`IValidator<TRequest>` in DI and called explicitly with `ValidateAndThrowAsync(...)` as the first line
of the relevant service method — not wired through automatic ASP.NET Core model validation. A
`FluentValidation.ValidationException` is caught by the middleware and turned into `400` with all
messages joined. Checks that require a database lookup (duplicate email, duplicate room name) belong in
the service, never in the validator — the validator only checks what's derivable from the request itself.

**Concurrency / duplicate-insert races**: the app-level "does this already exist?" check in a service
is inherently racy under concurrent requests (two requests can both pass the check before either
commits). The actual backstop is the database's unique index (e.g. `Users.Email`,
`MeetingRooms(Name, Location)`), and `ExceptionHandlingMiddleware` has a dedicated case that recognizes
a `DbUpdateException` wrapping a `SqlException` with error number `2601`/`2627` (unique violation) and
maps it to `409` — centrally, so every unique index in the app gets a clean conflict response instead of
a `500`, without per-service try/catch. This was verified with an actual concurrent-request test (fired
many simultaneous duplicate registrations; exactly one `201`, the rest `409`, exactly one DB row).

**Auth**: cookie-based (not JWT) — `AddAuthentication().AddCookie(...)` in `Program.cs`, with
`HttpOnly`, `SecurePolicy = Always` (cookie only works over **https**, i.e. the `:7175` profile — testing
over plain `:5062` silently fails to persist the session), and `SameSite = Lax`. `UserController.Login`
verifies credentials via `AuthService.Login`, then builds claims and calls `HttpContext.SignInAsync`
itself in the controller (the service layer has no `HttpContext` dependency by design).
`SameSite = Lax` was verified with a real cross-origin browser test (a form POST from a different
origin) to actually block the cookie on cross-site requests — confirmed working CSRF protection even
before any `[ValidateAntiForgeryToken]` is added.
`Register` always creates the `Employee` role — role is never taken from client input.
**There is currently no way to create an `Administrator` account** (no seeding exists yet), so
`[Authorize(Roles = "Administrator")]`-protected endpoints (e.g. `MeetingRoomController`) can't be
fully tested end-to-end until that's added.
`LoginPath`/`AccessDeniedPath` are deliberately left unconfigured for now (planned for when MVC pages
exist) — without them, an unauthorized cookie-auth request returns a plain `401`/`403` rather than
redirecting, which is what's wanted for a pure API today.

**Antiforgery**: `AddAntiforgery` is registered (`HeaderName = "X-CSRF-TOKEN"`) and
`AntiforgeryController.GetToken` (`GET /api/antiforgery/token`) issues a token + sets the cookie, but no
endpoint has `[ValidateAntiForgeryToken]` yet — that's deliberately deferred until later in the build.

**Logging**: Serilog (`Program.cs`), console + rolling daily file under `Logs/` (gitignored). No
bootstrap logger — just one `UseSerilog` call, since nothing logs before it runs and there's no
try/catch around startup to justify a separate bootstrap phase.

**Swagger**: Swashbuckle (not `Microsoft.AspNetCore.OpenApi`), with `IncludeXmlComments` wired to the
generated XML doc file, and `RouteOptions.LowercaseUrls = true` so routes match the lowercase paths
shown in each endpoint's XML `<remarks>` sample.

**Attribution**: do not add `Co-Authored-By`/Claude-generated mentions to git commits or PR
descriptions for this repository — the user has explicitly asked for plain, human-style messages.
