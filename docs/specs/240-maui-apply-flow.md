# 240 Desktop Apply Flow

## Goal
Run apply from UI and present execution report results.

## In scope
- Trigger apply for selected plan artifact.
- pre-requisiste - an existing plan has to have already been selected with the plan file uri present and available in the current context.
- DUring execution, display progress and final report summary.
- Surface abort/failure outcomes clearly.
- when applying, if the rename operation appears to already have been completed just skip over with logging to allow for continuation of previous plans that had to stop early - don't treat this as an error.  We want to allow idempotent re-runs.
- Map CLI/apply exit outcomes into UI error states.
- Desktop targets only (Windows + macOS Mac Catalyst).

## Out of scope
- Advanced report analytics.
- Background scheduling.
- iOS/Android/mobile/tablet targets.

## Implementation steps
1. Add apply command and progress state to apply ViewModel.
2. Invoke apply engine/service and obtain `RenameReport`.
3. Bind report summary/results into UI.
4. Map failure classes to UI messages:
   - validation/arg (`2`)
   - IO (`3`)
   - schema (`4`)
   - retry-abort (`5`)
   - unexpected (`6`)
5. Add tests for success/failure/abort state transitions.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~ApplyFlow"`

## Acceptance checks
- Successful apply shows summary counts and drift info.
- Retry-limit abort path is displayed with clear message.
- Schema/IO failures map to correct UI error states.

## Tests
- Add `src/Renamer.Tests/UI/ApplyFlowViewModelTests.cs`.

## Test scope
- Apply-flow ViewModel success/failure/abort behavior.

## Expected outputs
- Apply flow UI + ViewModel changes and tests.

## Exit criteria
- UI can execute and display report outcomes end-to-end.
