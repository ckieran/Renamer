# 260 Desktop Stepper Shell

## Goal
Introduce a step-navigation shell that moves the desktop UI from stacked left-side cards to a left-stepper plus right-workspace layout for generate, preview, and apply.

## In scope
- Replace the current left-column stack of functional cards with a compact step list:
  - `Generate Plan`
  - `Preview Plan`
  - `Apply Plan`
- Render only the active step's workspace in the right panel.
- Allow non-sequential navigation between steps.
- Add lightweight step status indicators for:
  - needs info
  - error
  - done
- Preserve existing plan-generation, plan-loading, preview, and apply behaviors while rearranging layout ownership.
- Keep desktop targets only (Windows + macOS Mac Catalyst).

## Out of scope
- Visual card restyling for operations or results.
- Changes to plan or report schema.
- Mobile/tablet targets.
- New rename workflow behavior beyond layout and step-state presentation.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c codex/260-maui-stepper-shell`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `codex/260-maui-stepper-shell`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Add a step selection model to the desktop UI ViewModel so `Generate Plan`, `Preview Plan`, and `Apply Plan` can be activated independently.
2. Refactor the desktop page layout so the left column is a step-navigation rail and the right column hosts the active step content.
3. Map existing workflow state into step status indicators for needs-info, error, and done without introducing sequential locking.
4. Keep current commands and bindings working while relocating controls into the active workspace structure.
5. Add or update tests that verify step selection and status mapping behavior.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~Stepper"`

## Git/PR workflow
0. Setup - change to `main` branch and pull latest from origin to prepare for new work.
1. Branch from current `main` using prefix `codex/` (for example `codex/260-maui-stepper-shell`).
2. Keep one slice per branch and one branch per PR.
3. Commit only files related to this slice.
4. If a matching GitHub issue exists, ensure it is `In Progress` before opening the PR.
5. Open PR back into `main` with slice ID in title/body.
6. Link the corresponding issue in the PR body:
   - use `Closes #<issue-number>` when the PR completes the slice
   - use `Refs #<issue-number>` only for partial work
7. Include the slice ID in PR title and body.

## Acceptance checks
- The desktop UI shows a left-side step navigation rail instead of stacked action cards.
- Selecting a step swaps the right-side workspace without requiring sequential completion.
- Each step shows a visible status affordance for needs-info, error, or done based on current workflow state.
- Existing generate, preview, and apply behaviors remain reachable after the layout change.

## Tests
- Add or update `src/Renamer.Tests/UI/*Stepper*.cs` coverage for step selection, default step, and status mapping.

## Test scope
- Desktop ViewModel and UI state behavior for step navigation shell only.

## Expected outputs
- Desktop UI shell layout updates for step navigation and active workspace hosting.
- ViewModel state for selected step and step status.
- Targeted tests for shell state behavior.

## Exit criteria
- The desktop UI uses a stepper-style shell while preserving existing workflow behavior.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are implemented and pass locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- Branch name follows `codex/` and maps to this slice ID.
- PR scope is limited to this slice (no unrelated refactors).
- If a matching GitHub issue exists, it is linked from the PR and moved to `In Progress` when the slice is picked up.
