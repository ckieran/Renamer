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

## Standard command sequence
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

## Slice execution rules
- Execute one slice per PR.
- Use the slice document as the sole scope for implementation.
- Run the commands listed in the slice plus the standard command sequence.

## PR Definition of Done
- Slice acceptance checks are satisfied.
- Required tests pass locally.
- `docs/checklists/v1.md` is updated for the slice.
- No unrelated refactors or drive-by changes.

## Test strategy guidance
- Prefer unit tests in early slices.
- Add CLI integration tests for `plan`/`apply` slices.
- For UI, prefer ViewModel/service-level tests plus targeted smoke checks.
