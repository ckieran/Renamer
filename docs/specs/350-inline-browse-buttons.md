# 350 Inline Browse Buttons

## Slice ID
`350-inline-browse-buttons`

## Goal
Demote per-field "browse" actions to small inline icon-sized buttons sitting beside their path fields, reserving the accent-colored primary button style for the single commit action of each screen.

## In scope
- Update `src/Renamer.UI/Views/GenerateWorkspaceView.xaml` so:
  - Each path `Entry` (root folder, output folder) lives in a `Grid` with `ColumnDefinitions="*,Auto"` and the browse `Button` sits in column 1 with a constrained width (~36px) and an icon-style label (folder glyph or short text label like "…").
  - The standalone full-width browse `Button`s are removed.
  - `GenerateButtonGenerateAndLoad` remains the only accent-colored primary button on this view.
- Update `src/Renamer.UI/Views/ApplyWorkspaceView.xaml` similarly if any browse / file-picker buttons in that view follow the same full-width pattern (audit during implementation; if none, note it in the PR description).
- Add a new `BrowseButton` style in `Resources/Styles` (or extend an existing button style) that fixes width, padding, and visual weight so the pattern is reusable.
- Add a glyph-only string resource if needed (e.g. `BrowseButtonGlyph` = "…" or a folder unicode character) to keep XAML free of hard-coded display text.
- Reuse the existing `Browse` resource keys (`GenerateBrowseRootFolder`, `GenerateBrowseOutputFolder`) for the button's `AutomationProperties.Name` / `ToolTipProperties.Text` so the action is still announced to assistive tech.

## Out of scope
- Default-folder behavior or `Advanced ▾` disclosure (slice `360`).
- Plan item card redesign (slice `370`).
- Theme / color tokens beyond what the new button style needs.
- Any change to the picker services (`IFolderPathPicker`, `IPlanFilePicker`) or the underlying commands.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c refactor/350-inline-browse-buttons`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `refactor/350-inline-browse-buttons`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1–6 are complete.

## Implementation steps
1. Add `BrowseButton` style to `src/Renamer.UI/Resources/Styles/` (or the existing styles file). Constrain width, padding, and visual weight; ensure it inherits the standard button base style for hover/focus consistency.
2. Refactor each path field block in `GenerateWorkspaceView.xaml` from a vertical stack `[Label][Entry][Button]` into `[Label]` + `Grid("*,Auto")` containing `[Entry][BrowseButton]`. Apply the new style. Bind `AutomationProperties.Name` and `ToolTipProperties.Text` to the existing `Browse*` resource keys.
3. Add `BrowseButtonGlyph` (or equivalent) to `AppStrings.resx` if a textual glyph is used. Regenerate `AppStrings.Designer.cs`.
4. Audit `ApplyWorkspaceView.xaml` for any browse buttons that fit the same pattern; apply the same restructure or note in PR that no changes were needed.
5. Visually verify the only accent-colored primary button on each screen is the commit action.
6. Update or add tests if any UI test asserts on the prior button arrangement (likely none; verify).

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet build Renamer.sln`
4. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `refactor/`.
2. Keep the PR scoped to button-style restructuring on the affected views and the supporting style/resource additions.
3. Title the PR with a Conventional Commit, for example `refactor(ui): 350 inline browse buttons`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- Each path `Entry` on the Generate workspace has a small inline browse button to its right; no full-width browse button remains.
- The Generate workspace shows exactly one accent-colored primary button: the commit action (`GenerateButtonGenerateAndLoad`).
- Browse buttons announce a name to assistive tech via `AutomationProperties.Name` from the existing `Browse*` resource keys.
- Apply workspace either follows the same pattern or the PR confirms no change was needed there.
- Browse, generate, and apply commands continue to fire correctly.

## Tests
- No new behavioral tests required. If any existing UI test asserts on the old button layout, update it.

## Test scope
- Standard `restore` / `build` / `test` must pass.

## Expected outputs
- `src/Renamer.UI/Views/GenerateWorkspaceView.xaml` — updated.
- `src/Renamer.UI/Views/ApplyWorkspaceView.xaml` — updated if applicable.
- `src/Renamer.UI/Resources/Styles/*.xaml` — new `BrowseButton` style.
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) — new glyph key if used.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Standard command sequence passes locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- PR scope limited to this slice.
