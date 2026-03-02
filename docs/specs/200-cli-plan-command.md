# 200 CLI Plan Command

## Goal
Implement CLI `plan` command to generate plan artifact.

## In scope
- Parse root path and output path inputs.
- Build plan using core services.
- Write `rename-plan.v1.json`.
- Return non-zero exit code on failure.

## Out of scope
- Apply execution.
- MAUI integration.

## Acceptance checks
- Command writes plan JSON successfully for valid inputs.
- Invalid inputs fail with non-zero exit code.

## Tests
- Command-level integration tests with temp directories.

## Exit criteria
- `plan` command is functional and tested.
