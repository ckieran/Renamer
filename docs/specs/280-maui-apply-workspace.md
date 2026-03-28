# 280 Desktop Apply Workspace

## Goal
Give the apply step a dedicated right-panel workspace that combines plan selection, apply execution, and inline results while prepopulating the most recently generated plan path.

## In scope
- Move apply-related controls into the `Apply Plan` workspace.
- Show plan artifact selection/loading controls at the top of the apply workspace.
- Prepopulate the apply plan path from the most recently generated plan when available.
- Keep support for applying a different previously generated or manually selected plan.
- Render apply status, error messaging, apply summary, and apply results inline below the load/execute controls.
- Keep desktop targets only (Windows + macOS Mac Catalyst).

## Out of scope
- Dedicated standalone results step.
- New execution semantics or retry policy changes.
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
   - `git switch -c codex/280-maui-apply-workspace`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `codex/280-maui-apply-workspace`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Move plan artifact load controls and apply action controls into the `Apply Plan` workspace.
2. Ensure plan generation success continues to hand off the generated plan path into the apply context by default.
3. Keep manual plan browsing/loading available so apply does not depend on the current session's generation path.
4. Reorganize apply summary, error state, and execution results to render inline under the load and apply controls.
5. Add or update tests for generated-plan handoff, apply validation, success, and failure visibility in the apply workspace.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~ApplyFlow|FullyQualifiedName~PlanGeneration"`

## Git/PR workflow
0. Setup - change to `main` branch and pull latest from origin to prepare for new work.
1. Branch from current `main` using prefix `codex/` (for example `codex/280-maui-apply-workspace`).
2. Keep one slice per branch and one branch per PR.
3. Commit only files related to this slice.
4. If a matching GitHub issue exists, ensure it is `In Progress` before opening the PR.
5. Open PR back into `main` with slice ID in title/body.
6. Link the corresponding issue in the PR body:
   - use `Closes #<issue-number>` when the PR completes the slice
   - use `Refs #<issue-number>` only for partial work
7. Include the slice ID in PR title and body.

## Acceptance checks
- The `Apply Plan` step shows plan selection/load controls and apply controls in the same right-side workspace.
- When a plan was just generated in the current session, the apply workspace defaults to that generated plan path.
- Users can still browse or paste a different plan artifact before loading and applying.
- Apply summary and apply results appear inline underneath the load and execute controls.
- Apply errors remain clear and actionable in the apply workspace.

## Tests
- Add or update `src/Renamer.Tests/UI/ApplyFlowViewModelTests.cs` and `src/Renamer.Tests/UI/PlanGenerationViewModelTests.cs` for apply workspace handoff and inline-state behavior.

## Test scope
- Apply workspace validation, generated-plan handoff, success state, and failure state behavior.

## Expected outputs
- Desktop apply workspace layout updates.
- ViewModel support for generated-plan handoff into apply context.
- Targeted tests for apply workspace behavior.

## Exit criteria
- Users can load and execute apply in a dedicated workspace with inline summary and results.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are implemented and pass locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- Branch name follows `codex/` and maps to this slice ID.
- PR scope is limited to this slice (no unrelated refactors).
- If a matching GitHub issue exists, it is linked from the PR and moved to `In Progress` when the slice is picked up.
