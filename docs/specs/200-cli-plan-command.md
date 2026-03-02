# 200 CLI Plan Command

## Goal
Implement CLI `plan` command to generate plan artifact.

## In scope
- Parse required args exactly:
  - `--root <path>`
  - `--out <path>`
- Validate args in contract order.
- Build plan using core services.
- Write `rename-plan.json` to output path (overwrite behavior).
- Return categorized non-zero exit code on failure.

## Out of scope
- Apply execution.
- MAUI integration.

## Implementation steps
1. Add command parser branch for `plan`.
2. Implement validation order from `080-cli-contract.md`.
3. Call plan builder and serializer.
4. Map failures to exit codes (`2`, `3`, `6`).
5. Add command integration tests.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet run --project src/Renamer.Cli -- plan --root ./samples --out /tmp/rename-plan.json`
4. `dotnet test Renamer.sln --filter "FullyQualifiedName~CliPlanCommand"`

## Acceptance checks
- Command writes plan JSON successfully for valid inputs.
- Invalid args return exit code `2`.
- Invalid root/writability issues return exit code `3`.

## Tests
- Add `src/Renamer.Tests/CLI/CliPlanCommandTests.cs`.

## Test scope
- Argument parsing, validation order, and exit-code mapping for `plan`.

## Expected outputs
- `plan` command implementation in CLI.
- CLI plan integration tests.

## Exit criteria
- `plan` command is functional and tested.
