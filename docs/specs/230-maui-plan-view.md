# 230 Desktop Plan View

## Goal
Render plan JSON in a user-friendly preview list.

## In scope
- Select/load plan artifact path.
- Deserialize and display operations list and summary values.
- UI states: idle, loading, loaded, error.
- Desktop targets only (Windows + macOS Mac Catalyst).

## Out of scope
- Executing apply.
- Editing plan contents.
- iOS/Android/mobile/tablet targets.

## Implementation steps
1. Add ViewModel for plan-loading workflow.
2. Implement JSON load using core serializer contract.
3. Map DTOs into display model.
4. Implement state transitions (idle/loading/loaded/error).
5. Bind view controls to ViewModel state.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanView"`

## Acceptance checks
- Valid plan file renders operations and summary.
- Invalid/missing file shows actionable error state.

## Tests
- Add `src/Renamer.Tests/UI/PlanViewModelTests.cs` for load/map/error states.

## Test scope
- Plan-view ViewModel behavior.

## Expected outputs
- Plan view UI bindings and ViewModel tests.

## Exit criteria
- Users can review planned operations in UI.
