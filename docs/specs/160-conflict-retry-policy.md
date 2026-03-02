# 160 Conflict Retry Policy

## Goal
Implement adaptive collision handling with deterministic suffix and max retries.

## In scope
- Suffix strategy ` (1)`..`(10)` after base destination.
- Max 10 retries on destination conflict.
- Abort current plan execution when unresolved.

## Out of scope
- JSON file writing.
- UI behavior.

## Acceptance checks
- Conflicts resolve to first available deterministic suffix.
- Unresolved conflict after retry limit aborts execution.

## Tests
- Unit tests for suffix order and max retry abort behavior.

## Exit criteria
- Retry policy is enforced and tested.
