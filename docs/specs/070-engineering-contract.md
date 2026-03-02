# Engineering Contract (Draft)

## Purpose
Define shared execution standards for every implementation slice so PRs are consistent and low-friction.

## Baseline stack
- Runtime and SDK: .NET 9
- Unit tests: xUnit
- Logging: Serilog
- EXIF parsing: MetadataExtractor

## Required behavior
- Core logic remains in `Renamer.Core`.
- CLI and UI are wrappers over core behavior.
- Logging is enabled from startup in each executable:
  - Console sink
  - One long-lived log file per executable

## Log path resolution contract
- Resolve through `ILogPathProvider`.
- Defaults:
  - Windows: `%LOCALAPPDATA%/Renamer/logs/<executable>.log`
  - macOS: `~/Library/Logs/Renamer/<executable>.log`

## Standard command sequence
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

## Exit code contract
- `0`: success
- `2`: invalid arguments or validation failure
- `3`: IO/path access failure
- `4`: unsupported or invalid plan schema
- `5`: conflict retry limit reached (plan execution abort)
- `6`: unexpected runtime error

## Slice execution rules
- Execute one slice per PR.
- Use the slice document as the sole scope for implementation.
- Run the commands listed in the slice plus the standard command sequence.

## Slice completion requirement
- Every slice must include:
  - implementation steps
  - commands to run
  - test scope
  - expected outputs

## PR Definition of Done
- Slice acceptance checks are satisfied.
- Required tests pass locally.
- `docs/checklists/v1.md` is updated for the slice.
- No unrelated refactors or drive-by changes.

## Test strategy guidance
- Prefer unit tests in early slices.
- Add CLI integration tests for `plan`/`apply` slices.
- For UI, prefer ViewModel/service-level tests plus targeted smoke checks.
