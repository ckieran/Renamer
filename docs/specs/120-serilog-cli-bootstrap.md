# 120 Serilog CLI Bootstrap

## Goal
Configure CLI operational logging from startup.

## In scope
- Add Serilog provider to CLI host.
- Write to console and long-lived CLI log file.
- Ensure info/error records are emitted for key flow points.

## Out of scope
- UI logging setup.
- Business logic changes.

## Acceptance checks
- CLI run creates/appends to log file.
- Errors are captured with exception details.

## Tests
- Smoke test or integration test verifying log file creation/appending.

## Exit criteria
- CLI logging is active from process start.
