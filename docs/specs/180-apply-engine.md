# 180 Apply Engine

## Goal
Execute rename operations from plan model with adaptive conflict behavior.

## In scope
- Attempt planned destination first.
- Retry with suffix policy on collision.
- Abort current plan on unresolved conflict.

## Out of scope
- CLI command handling.
- UI display.

## Acceptance checks
- Successful rename path updates status.
- Retry/abort behavior follows policy.

## Tests
- Unit tests with filesystem abstraction fakes.

## Exit criteria
- Apply engine behavior is reliable and tested.
