# 220 Serilog UI Bootstrap

## Goal
Configure MAUI UI operational logging from startup.

## In scope
- Add Serilog provider in MAUI startup.
- Write to console and long-lived UI log file.
- Capture startup and command flow info/error logs.

## Out of scope
- CLI logging setup.
- Feature-level UI behavior.

## Acceptance checks
- UI run creates/appends to UI log file.
- Unhandled operation errors are logged.

## Tests
- Manual smoke validation + targeted automated checks where feasible.

## Exit criteria
- UI logging is active from startup.

