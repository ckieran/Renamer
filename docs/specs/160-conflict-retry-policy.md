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

## Implementation steps
1. Add conflict policy helper that yields candidate paths in order.
2. Implement retry loop policy (initial path + 10 suffixed attempts max).
3. Return structured failure when retry limit is exceeded.
4. Map retry-limit failure to exit code `5` contract.
5. Add tests for exact suffix order and abort boundary.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~ConflictRetry"`

## Acceptance checks
- Conflicts resolve to first available deterministic suffix.
- Unresolved conflict after retry limit aborts execution.

## Tests
- Add `src/Renamer.Tests/Core/ConflictRetryPolicyTests.cs`.

## Test scope
- Conflict candidate generation and retry-limit behavior.

## Expected outputs
- Conflict policy helper and tests.

## Exit criteria
- Retry policy is enforced and tested.
