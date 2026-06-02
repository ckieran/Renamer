# 420 Plan Workspace Reconciliation

## Slice ID
`420-plan-workspace-reconciliation`

## Goal
Align the Plan workspace with the target concept: one compact folder row, secondary advanced defaults, bottom-left status text, and bottom-right primary action.

## In scope
- Update `src/Renamer.UI/Views/GenerateWorkspaceView.xaml` so the Plan workspace matches the target references:
  - `target-plan-defaults-collapsed.png`
  - `target-plan-advanced-expanded.png`
- Preserve inline browse behavior from slice `350`.
- Preserve smart defaults and advanced override behavior from slice `360`.
- Move the primary `Build plan` action into a bottom action area.
- Place discovered-folder/status text near the bottom-left action area.
- Reduce instructional copy to the minimum needed to operate the screen.
- Capture native Plan screenshots after implementation.

## Out of scope
- Top-level shell/header/frame changes (`410`).
- Review/Rename workspace changes (`430`).
- Plan generation behavior, serializers, filesystem IO, or validation rules.
- New advanced options beyond existing output folder / filename overrides.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean.
2. Verify current branch is `main`.
3. `git pull --ff-only origin main`.
4. `git switch -c refactor/420-plan-workspace-reconciliation`.
5. Confirm branch equals `refactor/420-plan-workspace-reconciliation`.
6. If a matching GitHub issue exists, move it to `In Progress`.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Compare the current native Plan screenshots with both target Plan references.
2. Rework `GenerateWorkspaceView.xaml` spacing and grouping so the path row, advanced disclosure, status, and action placement match the concept.
3. Keep command bindings and ViewModel behavior unchanged unless the layout needs a small display-only property.
4. Ensure light and dark themes remain legible.
5. Capture collapsed and expanded native Plan screenshots for PR notes.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanViewModel|FullyQualifiedName~Generate"`
4. `dotnet build Renamer.sln`
5. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from `main` with prefix `refactor/`.
2. Keep PR scoped to Plan workspace layout and directly required styles/resources.
3. PR title: `refactor(ui): 420 plan workspace reconciliation`.
4. Link matching issue with `Closes #<issue-number>` when one exists.

## Acceptance checks
- Plan workspace shows one compact photo-folder row with inline browse.
- Advanced defaults are secondary and collapsed by default unless overridden.
- `Build plan` appears as the single primary action in the bottom action area.
- Status/discovered-folder text appears near the bottom-left action area.
- Native screenshots are captured for collapsed and expanded states.

## Tests
- No new tests required unless ViewModel/display behavior changes.
- Existing Plan/Generate tests must continue to pass.

## Test scope
- Plan ViewModel and Generate workspace behavior.

## Expected outputs
- `src/Renamer.UI/Views/GenerateWorkspaceView.xaml`
- `src/Renamer.UI/Resources/Styles/*.xaml` if spacing/style helpers are needed.
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) if copy changes are needed.

## Definition of Done
- Acceptance checks satisfied.
- Required commands pass.
- Checklist updated.
- PR scope limited to this slice.
