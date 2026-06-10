# 440 Visual Polish Pass

## Slice ID
`440-visual-polish-pass`

## Goal
Close the visual gap recorded after slices `410`–`430`: legible native type scale, highlighted active rail step, breathing room in the rail, full-height workspaces with bottom-pinned action rows, constrained input width, and a readable theme control. Visuals only — no copy changes, no behavior changes beyond presentation.

## In scope
- Named text styles and a larger base type scale in `Resources/Styles/Styles.xaml`.
- Unified action-row control heights; larger theme pill in `MainPage.xaml`.
- `WorkflowRailView.xaml`: accent highlight on the active step's stacked-letter label, wider inter-step spacing.
- Workspace frames: each workspace fills the viewport with a bottom-pinned status/action row; proportional width cap (70%) on path input rows.
- Plan: Advanced disclosure stays collapsed after a path is entered (fix property-change ordering in output-directory autofill).
- Review: restyle existing stat pills toward the target's stat-box look (no new data plumbing).
- Rename: structured workspace (summary + pinned action row) replacing the single floating card.
- Rail indicator: do not show a step as Done ahead of the user reaching it (presentation-state mapping only).
- Native light/dark screenshots for Plan, Review, Rename in PR notes.

## Out of scope
- String/copy changes (`AppStrings.resx` values).
- Core/CLI changes, plan schema, persistence.
- New view models or workflow behavior.
- Pixel-perfect parity with mock absolute sizes (intentional deviation: larger native type scale).
- Step kicker line ("STEP 1 OF 3") and number-over-caption stat split — both need new strings, deferred to the copy pass.

## Implementation steps
1. Styles: named label styles (Kicker, WorkspaceTitle, Body, Caption); base size bump; unified button heights; theme pill sizing.
2. Rail: active-letter highlight bound to step indicator state; connector and letter spacing increases.
3. Workspace frame: Grid with `*` content row + pinned bottom action row per view; proportional input width cap.
4. Per-view reconciliation (Plan disclosure default, Rename structure, review-step Done gating).
5. Capture light/dark screenshots of all three views at 1100x720 and compare against `docs/specs/design-references/400-ui-reconciliation/` targets.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`
4. `dotnet build src/Renamer.UI/Renamer.UI.csproj -f net10.0-maccatalyst` (screenshot loop)

## Acceptance checks
- Active rail step shows accented circle and accented stacked-letter label; inactive steps stay muted.
- Rail steps have visibly more separation than the 410 baseline.
- Workspace panel occupies full height; status bottom-left, primary action bottom-right on every view.
- Path inputs no longer span the full 1100px window.
- Theme pill legible at a glance (≥14pt label).
- Advanced panel remains collapsed when a photo folder is typed.
- No `AppStrings.resx` value changes in the diff.

## Tests
- Existing unit/integration suites pass unchanged (`dotnet test Renamer.sln`).
- Visual acceptance via native screenshots (light + dark, all three workspaces).

## Test scope
- Standard restore/build/test gate; no new automated tests (XAML-only slice).

## Expected outputs
- Modified: `Styles.xaml`, `MainPage.xaml`, `WorkflowRailView.xaml`, `GenerateWorkspaceView.xaml`, `PreviewWorkspaceView.xaml`, `ApplyWorkspaceView.xaml` (+ code-behind where presentation state requires).
- `docs/checklists/v1.md` and `docs/specs/000-index.md` updated.
- PR `refactor(ui): 440 visual polish pass` with target refs, after-screenshots, and intentional deviations noted.

## Definition of Done
- Acceptance checks satisfied with screenshot evidence.
- Standard command sequence passes locally.
- Checklist `440` checked before PR merge.
- PR scoped to this slice only.
