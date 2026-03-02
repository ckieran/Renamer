# 180 Apply Engine

## Goal
Execute rename operations from plan model with adaptive conflict behavior.

## In scope
- Attempt planned destination first.
- Retry with suffix policy on collision.
- Abort current plan on unresolved conflict.
- Record attempt counts and per-operation result status.

## Out of scope
- CLI command handling.
- UI display.

## Implementation steps
1. Add `IApplyEngine` in core and concrete implementation.
2. For each operation, attempt rename to planned destination.
3. On conflict, apply `160` retry policy until success or retry limit.
4. If retry limit reached, mark current operation failed and abort plan execution.
5. Return result collection suitable for `RenameReport` serialization.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~ApplyEngine"`

## Acceptance checks
- Successful rename path updates status and attempts.
- Retry/abort behavior follows policy.
- Retry-limit failure maps to exit code contract `5` when surfaced by CLI.

## Tests
- Add `src/Renamer.Tests/Core/ApplyEngineTests.cs` with filesystem fakes.

## Test scope
- Apply engine success, retry success, retry abort.

## Expected outputs
- Apply engine implementation and tests.

## Exit criteria
- Apply engine behavior is reliable and tested.
