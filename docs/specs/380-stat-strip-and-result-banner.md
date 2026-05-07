# 380 Stat Strip and Result Banner

## Slice ID
`380-stat-strip-and-result-banner`

## Goal
Replace the verbose "Before you rename" (Created / Folder changes / Things to note) trio on the Preview workspace and the detailed Result / Started / Finished / Success / Skipped / Failed grid on the Apply workspace with compact stat-strip / banner summaries that lead with the headline number and tuck supporting details out of the way.

## In scope
- Preview workspace:
  - Replace the three "Before you rename" cards in `PreviewWorkspaceView.xaml` with a horizontal stat strip: small bordered tiles each showing a count + a short label (e.g. "2 changes", "0 notes").
  - If only one stat is non-zero, the strip should still render uniformly — no special-casing.
- Apply workspace:
  - Replace the detail grid in `ApplyWorkspaceView.xaml` with a single result banner: large headline (e.g. "Renamed 2 folders") and a one-line breakdown ("0 skipped · 0 failed").
  - Move start / finish timestamps and any other diagnostic detail behind a `Details ▾` disclosure (collapsed by default).
- Add display-derived properties on `PlanViewModel` if needed (e.g. `PreviewStatChanges`, `PreviewStatNotes`, `ApplyResultHeadline`, `ApplyResultBreakdown`).
- Update `AppStrings.resx` with shorter labels:
  - `PreviewStatChangesLabel` — e.g. `changes`
  - `PreviewStatNotesLabel` — e.g. `notes`
  - `ApplyResultHeadlineFormat` — e.g. `Renamed {0} folders` (with singular handling — see below)
  - `ApplyResultBreakdownFormat` — e.g. `{0} skipped · {1} failed`
  - `ApplyResultDetailsDisclosure` — e.g. `Details`
- Pluralization: use a small format helper or two resource keys (`...HeadlineSingular` / `...HeadlinePlural`) — pick one approach in implementation and stay consistent.

## Out of scope
- Plan item card redesign (`370`).
- Failure-mode messaging changes (covered by `330`).
- Persisting "Details ▾" expansion across launches.

## Pre-implementation gate (must pass before code edits)
1. Working tree clean.
2. On `main`.
3. `git pull --ff-only origin main`.
4. `git switch -c refactor/380-stat-strip-and-result-banner`.
5. Confirm branch.
6. Move matching issue to `In Progress`.
7. No code edits until steps 1–6 complete.

## Implementation steps
1. Audit `PlanViewModel` for existing summary fields. Add display-derived properties for the stat strip and result banner where missing.
2. Decide pluralization approach (format helper vs. dual resource keys). Document choice in PR description.
3. Update `PreviewWorkspaceView.xaml` to render the stat strip in place of the existing trio.
4. Update `ApplyWorkspaceView.xaml` to render the result banner; wrap the prior detail grid in a `Details ▾` disclosure.
5. Add resource keys to `AppStrings.resx`; regenerate Designer.
6. Add ViewModel tests for the new derived properties (zero, single, plural cases).

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanViewModel|FullyQualifiedName~ApplyFlow"`
4. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch with prefix `refactor/`.
2. PR scoped to preview/apply summary restructuring + supporting properties/tests.
3. PR title: `refactor(ui): 380 stat strip and result banner`.
4. Link issue with `Closes #<issue-number>`.

## Acceptance checks
- The Preview workspace shows a horizontal stat strip instead of three full cards.
- The Apply workspace shows a single headline + one-line breakdown, with full details available behind `Details ▾`.
- Pluralization reads correctly for 0, 1, and >1 in each stat.
- All copy is sourced from `AppStrings.resx`.
- No regressions in apply / preview behavior.

## Tests
- `src/Renamer.Tests/UI/PlanViewModelSummaryTests.cs` (new or extended): cover headline + breakdown derivation including plural cases.

## Expected outputs
- `src/Renamer.UI/Plans/PlanViewModel.cs` — derived summary properties.
- `src/Renamer.UI/Views/PreviewWorkspaceView.xaml` — stat strip.
- `src/Renamer.UI/Views/ApplyWorkspaceView.xaml` — result banner + `Details ▾` disclosure.
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) — new keys.
- `src/Renamer.Tests/UI/PlanViewModelSummaryTests.cs` — tests.

## Definition of Done
- Acceptance checks satisfied.
- Tests pass.
- Checklist updated.
- Pre-implementation gate completed.
- PR scope limited to this slice.
