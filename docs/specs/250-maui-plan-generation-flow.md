# 250 Desktop Plan Generation Flow

## Goal
Generate a plan from the desktop UI by selecting a root folder and output plan artifact path.

## In scope
- Select a root folder from the UI for plan generation.
- Select or enter the output path for `rename-plan.json`.
- Validate required inputs before generation.
- Invoke core plan-building and plan-serialization services from the UI.
- Surface progress and success/error states for plan generation.
- After a successful generate operation, populate the current plan context with the generated plan artifact path so the existing preview/apply flow can continue without re-selection.
- Desktop targets only (Windows + macOS Mac Catalyst).

## Out of scope
- Editing generated plan contents in the UI.
- Background or scheduled generation.
- Mobile/tablet targets.
- Changes to plan JSON schema or core naming rules.

## Implementation steps
1. Add root-folder and output-path selection state to the desktop plan ViewModel.
2. Add UI commands for browsing the root folder and choosing the output plan artifact path.
3. Invoke plan builder and serializer from the UI layer with validation and progress state.
4. On success, set the generated plan artifact path as the active plan path and make it available to the existing preview flow.
5. Add tests for validation, success handoff, and failure-state mapping.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanGeneration"`

## Acceptance checks
- Users can select a root folder and output plan location from the desktop UI.
- Successful generation writes `rename-plan.json` and makes it available to the existing preview workflow without manual re-entry.
- Missing/invalid paths show actionable UI error states.
- IO failures during generation map to clear UI messages.

## Tests
- Add `src/Renamer.Tests/UI/PlanGenerationViewModelTests.cs`.

## Test scope
- Plan-generation ViewModel validation, success handoff, and error-state behavior.

## Expected outputs
- Desktop UI plan-generation flow changes and tests.

## Exit criteria
- UI can generate a plan artifact end-to-end and hand off to the existing preview/apply context.
