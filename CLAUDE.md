# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project state

This is currently the unmodified output of `dotnet new webapi` (ASP.NET Core, .NET 10, controller-based).
The only code present is the default `WeatherForecastController` sample — no hotel booking domain
model, data access, or business logic has been implemented yet. There is no git repository initialized
and no README. Treat any "architecture" here as the starting scaffold, not an established pattern to
preserve — when the user starts adding real features, this file should be updated to describe them.

## Solution layout

- `HotelBooking.slnx` — solution file (new XML-based slnx format) referencing the single project.
- `HotelBooking/HotelBooking.csproj` — the Web API project (`Microsoft.NET.Sdk.Web`, target framework `net10.0`, nullable + implicit usings enabled).

## Common commands

Run all commands from the repo root (`C:\Users\sergi\source\repos\HotelBooking`) unless noted.

```bash
# restore dependencies
dotnet restore

# build
dotnet build

# run the API (uses launchSettings.json profiles: http on :5062, https on :7175/:5062)
dotnet run --project HotelBooking/HotelBooking.csproj
```

There is no test project yet. When one is added (typically `HotelBooking.Tests` alongside
`HotelBooking/`), add the corresponding `dotnet test` invocation here, including how to run a single
test (`dotnet test --filter FullyQualifiedName~TestName`).

## Notes for future work

- OpenAPI is wired up via `Microsoft.AspNetCore.OpenApi` and only mapped in Development
  (`app.MapOpenApi()` inside the `IsDevelopment()` check in [Program.cs](HotelBooking/Program.cs)).
- The project uses attribute-routed controllers (`[ApiController]`, `[Route("[controller]")]`), not
  Minimal APIs — follow that convention for new endpoints unless the user asks to switch styles.
