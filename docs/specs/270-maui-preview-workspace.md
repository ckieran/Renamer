# 270 Desktop Preview Workspace

## Goal
Give the preview step a dedicated right-panel workspace that presents plan summary first and planned operations underneath, using the fuller desktop width.

## In scope
- Move plan preview content into the `Preview Plan` workspace.
- Show plan summary as a breakout section above the operations list.
- Keep operations browsing as the primary detailed content below the summary.
- Retain preview states:
  - idle
  - loading
  - loaded
  - error
- Keep summary and operations bound to the existing plan artifact load workflow.
- Keep desktop targets only (Windows + macOS Mac Catalyst).

## Out of scope
- Apply execution controls or apply results layout.
- New filtering, sorting, or search behavior for operations.
- Visual card restyling beyond layout regrouping.
- Mobile/tablet targets.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c codex/270-maui-preview-workspace`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `codex/270-maui-preview-workspace`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Move the plan artifact load controls and preview messaging into the `Preview Plan` workspace.
2. Reorganize loaded preview content so summary appears as a breakout section above operations.
3. Ensure idle, loading, loaded, and error states render within the preview workspace without relying on the old stacked page structure.
4. Reuse existing plan-loading logic and data models rather than introducing new preview contracts.
5. Add or update tests for preview workspace state rendering and summary-plus-operations visibility.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanView|FullyQualifiedName~Preview"`

## Git/PR workflow
0. Setup - change to `main` branch and pull latest from origin to prepare for new work.
1. Branch from current `main` using prefix `codex/` (for example `codex/270-maui-preview-workspace`).
2. Keep one slice per branch and one branch per PR.
3. Commit only files related to this slice.
4. If a matching GitHub issue exists, ensure it is `In Progress` before opening the PR.
5. Open PR back into `main` with slice ID in title/body.
6. Link the corresponding issue in the PR body:
   - use `Closes #<issue-number>` when the PR completes the slice
   - use `Refs #<issue-number>` only for partial work
7. Include the slice ID in PR title and body.

## Acceptance checks
- The `Preview Plan` step shows plan artifact load controls in the right workspace.
- When a plan is loaded, summary is shown above operations instead of in a separate left-side card.
- Preview idle, loading, and error states are contained within the preview workspace.
- Operations remain readable and scrollable in the wider right-side content area.

## Tests
- Add or update `src/Renamer.Tests/UI/PlanViewModelTests.cs` and any new preview-workspace coverage needed for state visibility.

## Test scope
- Preview workspace load, summary display, operations display, and error-state behavior.

## Expected outputs
- Desktop preview workspace layout updates.
- Adjusted bindings for preview controls, summary, and operations placement.
- Targeted tests for preview workspace behavior.

## Exit criteria
- Users can load and review a plan in a dedicated preview workspace with summary first and operations second.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are implemented and pass locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- Branch name follows `codex/` and maps to this slice ID.
- PR scope is limited to this slice (no unrelated refactors).
- If a matching GitHub issue exists, it is linked from the PR and moved to `In Progress` when the slice is picked up.
