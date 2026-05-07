# 340 Thin Rail Redesign

## Slice ID
`340-thin-rail-redesign`

## Goal
Replace the full-card workflow rail with a thin numbered-indicator rail that uses a circular step number, an explicit visual state (done / now / next / error), and a vertical stacked-letter step label, freeing horizontal space for the active workspace.

## In scope
- Rewrite `src/Renamer.UI/Views/WorkflowRailView.xaml` so each step is rendered as:
  - A circular numbered indicator (~32–36px diameter) showing the step number, with `VisualStateManager` (or `DataTrigger`) variants for `Done`, `Now`, `Next`, and `Error`.
  - A vertical label rendered as a stack of single-character `Label`s (e.g. `P / L / A / N`) in a `VerticalStackLayout`, sourced from `PlanWorkflowStepItem.Title`.
  - A short connector line (`BoxView`) between adjacent indicators.
  - The whole step is still tappable (existing `TapGestureRecognizer` + `SelectStepCommand` preserved).
- Update `src/Renamer.UI/MainPage.xaml` so the rail column width drops from `280` to `72`.
- Add a derived property on `PlanWorkflowStepItem` that exposes the title as `IReadOnlyList<string>` (one element per character) for binding to a `BindableLayout`-backed `VerticalStackLayout`. Naming suggestion: `TitleCharacters`.
- Preserve current state semantics:
  - `IsSelected = true` ⇒ visual state `Now` (orange/accent fill).
  - `Status = Done` ⇒ visual state `Done` (success fill, checkmark glyph instead of number).
  - `Status = Error` ⇒ visual state `Error` (warning stroke, number kept).
  - Otherwise ⇒ visual state `Next` (outline only, muted text).
- Update affected XAML-bound resources in `Resources/Themes/*.xaml` if a new color key is required (e.g. `RailIndicatorBackgroundActive`, `RailIndicatorBackgroundDone`); reuse existing keys where possible.

## Out of scope
- Any change to `IPlanViewModel` step selection logic, `SelectStepCommand`, or `PlanWorkflowStep` enum values.
- Workspace-side layout changes (Generate / Preview / Apply views).
- Browse-button restyling (slice `350`).
- Defaults / Advanced disclosure (slice `360`).
- Plan item card redesign (slice `370`).
- Theme toggle compaction (slice `390`).
- Localization-aware vertical labels for non-Latin scripts (called out in `335`; deferred).
- Mobile/tablet layouts.

## Dependency notes
- Depends on `260-maui-stepper-shell.md` (existing rail + step model).
- Depends on the design decisions in `335-ui-cleanup-design-baseline.md`.
- Should not regress copy decisions from `300/310/320/330` — all displayed text continues to come from `AppStrings.resx`.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c refactor/340-thin-rail-redesign`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `refactor/340-thin-rail-redesign`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1–6 are complete.

## Implementation steps
1. Add `TitleCharacters` (or equivalently named) read-only property to `PlanWorkflowStepItem` that returns the step title split into per-character strings. Trim whitespace and uppercase the result for consistent display. Notify on change only if `Title` ever becomes mutable (currently it does not).
2. Rewrite `WorkflowRailView.xaml` to render each step as `[indicator] [vertical-letter-label]` inside a `VerticalStackLayout` of three steps separated by short connector lines. Apply a `Style`-driven approach to indicator visuals using `VisualStateManager` keyed off a step-state string, or `DataTrigger`s keyed off `IsSelected` and `Status`.
3. Update `MainPage.xaml` `ColumnDefinitions` for the main grid: `72,*` instead of `280,*`. Adjust `Padding` / `Margin` only as needed to keep the workspace content from butting against the rail.
4. Audit `Resources/Themes/*.xaml` for color keys used by the existing rail; either reuse them or add minimally-named new keys (`RailIndicatorBackgroundActive`, etc.) and update both `Light` and `Dark` themes.
5. Add or update `src/Renamer.Tests/UI/*Stepper*` (or a new `WorkflowRailItemTests.cs`) to cover:
   - `TitleCharacters` returns one entry per character of the trimmed/uppercased title.
   - State→indicator mapping is correct for combinations of `IsSelected` and `Status`.
6. Visually verify on Windows (and macOS if available) that the rail is ~72px wide, indicators are circular, and the stacked letters read correctly.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build src/Renamer.UI/Renamer.UI.csproj`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~Stepper|FullyQualifiedName~WorkflowRail"`
4. `dotnet build Renamer.sln`
5. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `refactor/`.
2. Keep the PR scoped to the rail rewrite, the supporting `PlanWorkflowStepItem` property, the column width change in `MainPage.xaml`, theme color keys, and the related tests.
3. Title the PR with a Conventional Commit, for example `refactor(ui): 340 thin numbered rail`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- The desktop UI shows a vertical rail roughly 72px wide on the left with three numbered circular indicators and stacked-letter step labels.
- The active step's indicator is filled with the accent color; completed steps show a checkmark on a success fill; future steps are outlined in muted color.
- Tapping a rail step still selects that step and swaps the right-hand workspace, matching prior behavior.
- All step titles continue to be sourced from `AppStrings.resx` — no hard-coded English in `WorkflowRailView.xaml`.
- The right-hand workspace gains horizontal space (column width changed from 280 to 72) without overflowing.

## Tests
- New or updated `src/Renamer.Tests/UI/WorkflowRailItemTests.cs` covering `TitleCharacters` derivation.
- Updated `src/Renamer.Tests/UI/*Stepper*.cs` if existing tests assert on the old card layout structure (they should not — these are ViewModel-level — but verify).

## Test scope
- `PlanWorkflowStepItem.TitleCharacters` derivation.
- Existing stepper selection / status mapping tests must still pass unchanged.

## Expected outputs
- `src/Renamer.UI/Views/WorkflowRailView.xaml` — rewritten.
- `src/Renamer.UI/Plans/PlanWorkflowStepItem.cs` — new `TitleCharacters` property.
- `src/Renamer.UI/MainPage.xaml` — column width updated.
- `src/Renamer.UI/Resources/Themes/Light.xaml` and `Dark.xaml` — color key updates if required.
- `src/Renamer.Tests/UI/WorkflowRailItemTests.cs` — new tests covering the derived property and state mapping.

## Exit criteria
- The rail visually matches the thin-numbered-rail direction in `335-ui-cleanup-design-baseline.md`.
- Stepper behavior (selection, status badges) is unchanged at the ViewModel level.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are implemented and pass locally.
- Standard command sequence (`restore` / `build` / `test`) passes.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- Branch name follows `refactor/` and maps to this slice ID.
- PR scope is limited to this slice (no unrelated refactors).
- If a matching GitHub issue exists, it is linked from the PR and moved to `In Progress` when the slice is picked up.
