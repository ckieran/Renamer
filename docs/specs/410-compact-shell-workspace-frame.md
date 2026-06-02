# 410 Compact Shell and Workspace Frame

## Slice ID
`410-compact-shell-workspace-frame`

## Goal
Align the native app shell with the compact concept mockup by replacing the landing-scale header and large framed canvas with a denser app header plus rail/workspace layout.

## In scope
- Update `src/Renamer.UI/MainPage.xaml` shell layout so:
  - The header title is compact (`Renamer`) with `Plan · Review · Rename` as supporting breadcrumb text.
  - Step-specific page titles remain inside the active workspace, not in a large top-level hero.
  - Page padding and vertical whitespace are reduced to match the 1100x720 target references.
  - The rail and workspace read as peers in the main app surface.
  - Unnecessary outer framing around the full workspace is removed or visually demoted.
- Keep the compact theme toggle from slice `390`.
- Keep rail behavior from slice `340`.
- Capture native screenshots after implementation for Plan light/dark at minimum.

## Out of scope
- Generate workspace field/action rearrangement beyond what is necessary to fit the shell (`420`).
- Review/Rename card, stat, or result-banner changes (`430`).
- Theme color token overhaul.
- Core, CLI, serializer, or plan/apply behavior.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main`:
   - `git switch -c refactor/410-compact-shell-workspace-frame`
5. Confirm branch:
   - `git branch --show-current` equals `refactor/410-compact-shell-workspace-frame`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Compare `current-plan-dark.png` / `current-plan-light.png` with `target-plan-defaults-collapsed.png`.
2. Update the root page grid/header in `MainPage.xaml` to remove landing-scale copy and reduce outer padding.
3. Adjust rail/workspace column spacing and top alignment without changing step selection behavior.
4. Keep all visible copy sourced from `AppStrings.resx`; add or reuse resource keys as needed.
5. Run the native app and capture Plan light/dark screenshots for the PR notes.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet build Renamer.sln`
4. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from `main` with prefix `refactor/`.
2. Keep PR scoped to shell/header/frame layout and directly required resources/styles.
3. PR title: `refactor(ui): 410 compact shell and workspace frame`.
4. Link matching issue with `Closes #<issue-number>` when one exists.

## Acceptance checks
- Top-level header no longer says `Rename your folders in three steps`.
- Top-level header uses compact `Renamer` + workflow breadcrumb treatment.
- Active step heading remains inside the workspace.
- Rail and workspace align near the top of the main work area.
- Plan light/dark screenshots are captured and referenced in the PR notes.

## Tests
- No new behavioral tests required unless resource or ViewModel wiring changes.

## Test scope
- Standard restore/build/test must pass.

## Expected outputs
- `src/Renamer.UI/MainPage.xaml`
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) if new copy keys are needed.
- `src/Renamer.UI/Resources/Styles/*.xaml` or theme files only if shell spacing/style tokens require it.

## Definition of Done
- Acceptance checks satisfied.
- Required commands pass.
- Checklist updated.
- PR scope limited to this slice.
