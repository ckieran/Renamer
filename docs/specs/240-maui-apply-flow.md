# 240 MAUI Apply Flow

## Goal
Run apply from UI and present execution report results.

## In scope
- Trigger apply for selected plan artifact.
- Display progress and final report summary.
- Surface abort/failure outcomes clearly.
- Map CLI/apply exit outcomes into UI error states.

## Out of scope
- Advanced report analytics.
- Background scheduling.

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
2. `dotnet build Renamer.sln`
3. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
4. `dotnet test Renamer.sln --filter "FullyQualifiedName~ApplyFlow"`

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
