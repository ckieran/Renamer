# 370 Plan Items as Cards

## Slice ID
`370-plan-items-as-cards`

## Goal
Restructure the preview list so each proposed folder rename is a single bounded card with a clear from→to pair and a status pill, replacing the current row layout where labels and values float independently.

## In scope
- Update `src/Renamer.UI/Views/PreviewWorkspaceView.xaml` so the `CollectionView` (or equivalent) `ItemTemplate` for `PlanOperationItem` renders:
  - A `Border` with rounded corners as the card surface.
  - A two-line stack: the original folder name in monospace muted text on top, the proposed new name in monospace primary text on the second line.
  - A status pill on the right (color-coded by `PlanOperationItem` state — ok / warning / conflict / skipped, etc., matching the existing operation states).
  - Item spacing of 8–12px between cards.
- Add or extend a `PlanOperationCard` style in `Resources/Styles`.
- If `PlanOperationItem` lacks display-friendly properties for the from/to pair or the status pill text/color, add derived properties on the item (e.g. `FromDisplay`, `ToDisplay`, `StatusPillText`, `StatusPillColor`).
- Keep all displayed text sourced from `AppStrings.resx` where applicable; the from/to values themselves remain data.

## Out of scope
- Stat strip / "Before you rename" replacement (slice `380`).
- Apply-result row redesign (slice `380`).
- Any change to plan generation, conflict logic, or the underlying `PlanOperationItem` semantics beyond adding display-derived properties.
- Selection / bulk-action UI on the card (deferred).

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean.
2. Verify current branch is `main`.
3. Update local `main` from origin.
4. `git switch -c refactor/370-plan-items-as-cards`.
5. Confirm branch name.
6. Move matching GitHub issue to `In Progress` if present.
7. Do not edit code until steps 1–6 are complete.

## Implementation steps
1. Audit `PlanOperationItem` for existing display fields. Add `FromDisplay`, `ToDisplay`, `StatusPillText`, `StatusPillColor` if missing — derived from existing state.
2. Write `PlanOperationCard` style (background, border, corner radius, padding) in `Resources/Styles`.
3. Rewrite the `ItemTemplate` in `PreviewWorkspaceView.xaml` to use the new card layout: `Grid("*,Auto")` with the from/to stack on the left and the status pill on the right.
4. Ensure the card surface respects existing theme color keys (light/dark) — add new keys only if needed.
5. Add unit tests for the new derived properties on `PlanOperationItem` covering each operation state.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanOperation|FullyQualifiedName~Preview"`
4. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from `main` with prefix `refactor/`.
2. Keep PR scoped to preview list restructuring and supporting derived properties + tests.
3. PR title: `refactor(ui): 370 plan items as cards`.
4. Link matching issue with `Closes #<issue-number>`.

## Acceptance checks
- Each proposed folder rename in the preview is shown as a single bounded card.
- The card shows the original name above the proposed name, both in monospace, with the proposed name visually dominant.
- Each card has a status pill on the right whose color and text reflect the operation state.
- Light and dark themes both render cards legibly.
- No regressions in plan generation, loading, or preview population.

## Tests
- `src/Renamer.Tests/Plans/PlanOperationItemDisplayTests.cs` (new) covering each operation state's pill text + color and the from/to display strings.
- Existing preview / plan loading tests must continue to pass.

## Expected outputs
- `src/Renamer.UI/Plans/PlanOperationItem.cs` — display-derived properties added.
- `src/Renamer.UI/Views/PreviewWorkspaceView.xaml` — card-based item template.
- `src/Renamer.UI/Resources/Styles/*.xaml` — `PlanOperationCard` style.
- `src/Renamer.Tests/Plans/PlanOperationItemDisplayTests.cs` — new tests.

## Definition of Done
- Acceptance checks satisfied.
- Tests pass.
- Checklist updated.
- Pre-implementation gate completed.
- PR scope limited to this slice.
