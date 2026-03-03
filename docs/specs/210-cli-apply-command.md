# 210 CLI Apply Command

## Goal
Implement CLI `apply` command to execute plan and emit report artifact.

## In scope
- Parse required args exactly:
  - `--plan <path>`
  - `--out <path>`
- Validate args in contract order.
- Execute apply engine.
- Write `rename-report.json` to output path (overwrite behavior).
- Return categorized non-zero exit code on failure/abort.

## Out of scope
- Plan creation logic.
- UI integration.

## Implementation steps
1. Add command parser branch for `apply`.
2. Implement validation order from `080-cli-contract.md`.
3. Validate schema version and load plan.
4. Execute apply engine and serialize report.
5. Map failures to exit codes (`2`, `3`, `4`, `5`, `6`).

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet run --project src/Renamer.Cli -- apply --plan /tmp/rename-plan.json --out /tmp/rename-report.json`
4. `dotnet test Renamer.sln --filter "FullyQualifiedName~CliApplyCommand"`

## Acceptance checks
- Valid plan executes and produces report.
- Unsupported schema returns exit code `4`.
- Retry-limit abort returns exit code `5` and failed report.

## Tests
- Add `src/Renamer.Tests/CLI/CliApplyCommandTests.cs`.

## Test scope
- Argument parsing, schema validation, apply execution, and exit-code mapping.

## Expected outputs
- `apply` command implementation in CLI.
- CLI apply integration tests.

## Exit criteria
- `apply` command is functional and tested.
