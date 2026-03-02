# 210 CLI Apply Command

## Goal
Implement CLI `apply` command to execute plan and emit report artifact.

## In scope
- Parse plan path and report output path.
- Execute apply engine.
- Write `rename-report.v1.json`.
- Return non-zero exit code on failure/abort.

## Out of scope
- Plan creation logic.
- MAUI integration.

## Acceptance checks
- Valid plan executes and produces report.
- Retry limit abort produces failed report and non-zero exit.

## Tests
- Command-level integration tests for success and conflict-abort paths.

## Exit criteria
- `apply` command is functional and tested.
