# 430 Review and Rename Workspace Reconciliation

## Slice ID
`430-review-rename-workspace-reconciliation`

## Goal
Align the Review and Rename workspaces with the target concept density, action placement, and summary/card hierarchy.

## In scope
- Update `src/Renamer.UI/Views/PreviewWorkspaceView.xaml` and `src/Renamer.UI/Views/ApplyWorkspaceView.xaml` as needed so Review/Rename match the reconciliation direction.
- For Review:
  - Preserve the stat strip and operation cards from slices `370` and `380`.
  - Align card density, from/to hierarchy, status pill placement, and bottom action row with `target-review.png`.
- For Rename:
  - Preserve apply gating and report behavior.
  - Use the same shell/workspace/action-row conventions as Plan and Review.
  - Keep result summary compact and avoid large explanatory blocks.
- Capture native Review and Rename screenshots after implementation.

## Out of scope
- Top-level shell/header/frame changes (`410`).
- Plan workspace changes (`420`).
- Plan/apply core behavior, serializers, filesystem IO, or report schema.
- New selection, filtering, or bulk edit features.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean.
2. Verify current branch is `main`.
3. `git pull --ff-only origin main`.
4. `git switch -c refactor/430-review-rename-workspace-reconciliation`.
5. Confirm branch equals `refactor/430-review-rename-workspace-reconciliation`.
6. If a matching GitHub issue exists, move it to `In Progress`.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Compare `current-review-dark.png` with `target-review.png`.
2. Compare `current-rename-dark.png` with the Rename view in `hifi-mocks-v2.jsx`.
3. Adjust Preview workspace spacing, card hierarchy, and action row without changing plan loading behavior.
4. Adjust Apply workspace spacing, result banner density, and action row without changing apply behavior.
5. Ensure light and dark themes remain legible.
6. Capture native Review and Rename screenshots for PR notes.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanOperation|FullyQualifiedName~Preview|FullyQualifiedName~Apply"`
4. `dotnet build Renamer.sln`
5. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from `main` with prefix `refactor/`.
2. Keep PR scoped to Review/Rename workspace layout and directly required styles/resources.
3. PR title: `refactor(ui): 430 review and rename workspace reconciliation`.
4. Link matching issue with `Closes #<issue-number>` when one exists.

## Acceptance checks
- Review workspace matches the target stat/card density and bottom action row.
- Operation cards keep clear from/to hierarchy and status pill placement.
- Rename workspace uses compact apply/result sections and shared action-row conventions.
- Native Review and Rename screenshots are captured and referenced in the PR notes.
- No regression in loading, continuing, applying, or report display behavior.

## Tests
- No new tests required unless display-derived properties or resource wiring changes.
- Existing Preview/Apply tests must continue to pass.

## Test scope
- Preview operation display and Apply workspace behavior.

## Expected outputs
- `src/Renamer.UI/Views/PreviewWorkspaceView.xaml`
- `src/Renamer.UI/Views/ApplyWorkspaceView.xaml`
- `src/Renamer.UI/Resources/Styles/*.xaml` if spacing/style helpers are needed.
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) if copy changes are needed.

## Definition of Done
- Acceptance checks satisfied.
- Required commands pass.
- Checklist updated.
- PR scope limited to this slice.
