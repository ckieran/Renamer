# 220 Serilog UI Bootstrap

## Goal
Configure MAUI UI operational logging from startup.

## In scope
- Add Serilog provider in MAUI startup.
- Resolve OS-aware long-lived UI log path via `ILogPathProvider`.
- Write to console and `renamer-ui.log`.
- Capture startup and command flow info/error logs.

## Out of scope
- CLI logging setup.
- Feature-level UI behavior.

## Implementation steps
1. Add/confirm Serilog packages in `Renamer.UI`.
2. Register `ILogPathProvider` in UI DI container.
3. Resolve log path:
   - Windows `%LOCALAPPDATA%/Renamer/logs/renamer-ui.log`
   - macOS `~/Library/Logs/Renamer/renamer-ui.log`
4. Configure logger before app services initialization.
5. Ensure unhandled exceptions are logged.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
4. `dotnet test Renamer.sln --filter "FullyQualifiedName~UiLogging"`

## Acceptance checks
- UI run creates/appends to long-lived `renamer-ui.log`.
- Unhandled operation errors are logged.

## Tests
- Add `src/Renamer.Tests/UI/UiLoggingTests.cs` where feasible; supplement with manual smoke check.

## Test scope
- UI logging bootstrap and path resolution.

## Expected outputs
- UI logging configuration and any supporting tests.

## Exit criteria
- UI logging is active from startup.
