# 120 Serilog CLI Bootstrap

## Goal
Configure CLI operational logging from startup with OS-aware log paths.

## In scope
- Add Serilog provider to CLI startup.
- Implement `ILogPathProvider` for OS-aware defaults.
- Write to console and long-lived log file `renamer-cli.log`.

## Out of scope
- UI logging setup.
- Business logic changes.

## Implementation steps
1. Add Serilog package references to `Renamer.Cli` as needed.
2. Add `ILogPathProvider` interface in core and default implementation in CLI wiring.
3. Resolve default log path:
   - Windows `%LOCALAPPDATA%/Renamer/logs/renamer-cli.log`
   - macOS `~/Library/Logs/Renamer/renamer-cli.log`
4. Configure logger at process start before command dispatch.
5. Emit startup and fatal-exception logs.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet run --project src/Renamer.Cli -- plan --root ./samples --out /tmp/rename-plan.json`
4. `dotnet test Renamer.sln --filter "FullyQualifiedName~CliLogging"`

## Acceptance checks
- CLI creates or appends to long-lived `renamer-cli.log`.
- Startup and error paths produce structured log entries.

## Tests
- Add `src/Renamer.Tests/CLI/CliLoggingTests.cs` for log path and log file creation.

## Test scope
- CLI logging tests only.

## Expected outputs
- CLI startup logging configuration.
- Log path provider implementation and tests.

## Exit criteria
- CLI logging is active from process start.
