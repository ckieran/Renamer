# 360 Plan File Smart Defaults

## Slice ID
`360-plan-file-smart-defaults`

## Goal
Stop forcing the user to specify a save folder and plan filename for the common case. Default the output folder to the chosen photo folder, default the filename to `rename-plan.json`, and hide both fields behind an `Advanced ▾` disclosure that only matters when the user wants to override.

## In scope
- Update `Renamer.UI.Plans.PlanViewModel`:
  - When `GenerationRootPath` changes and the user has not explicitly set `GenerationOutputDirectoryPath`, default the output directory to the root path.
  - Default `PlanFileName` to `rename-plan.json` when not set.
  - Track whether the user has overridden either default (so that further root-path changes do not stomp a user-chosen output path).
  - Surface a single boolean property `HasAdvancedOverrides` (true when either default has been overridden) for the disclosure to bind to.
- Update `src/Renamer.UI/Views/GenerateWorkspaceView.xaml`:
  - Wrap the output-folder + filename + computed-path-preview block in a collapsible region (e.g. an `Expander` if available in the project's MAUI toolkit, otherwise a `Button` toggling `IsVisible` on a `VerticalStackLayout`).
  - Default the disclosure to collapsed.
  - Auto-expand when `HasAdvancedOverrides` is true (e.g. after the user opens it once and edits a value).
  - Add a small helper `Label` showing the resolved plan-save location in collapsed state, e.g. "Plan will be saved beside your photos."
- Add new resource keys to `AppStrings.resx`:
  - `GenerateAdvancedDisclosureLabel` — e.g. `Advanced`
  - `GenerateAdvancedDisclosureHelpCollapsed` — e.g. `Plan will be saved beside your photos.`
  - `DefaultPlanFileName` — `rename-plan.json` (referenced by the ViewModel default; living in resx so the default file *name* is localizable even though the default value is the same in English).
- Update tests:
  - ViewModel default behavior: setting `GenerationRootPath` updates `GenerationOutputDirectoryPath` only when not overridden.
  - Filename defaults to `rename-plan.json` when no value supplied.
  - Override flags transition correctly.

## Out of scope
- Visual restyling of the disclosure beyond the simple show/hide pattern.
- Plan item card redesign (`370`).
- Validation rule changes for output folder / plan file (no behavior change in success/error cases beyond defaulting).
- Persisting overrides across app launches.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c feat/360-plan-file-smart-defaults`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `feat/360-plan-file-smart-defaults`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1–6 are complete.

## Implementation steps
1. Introduce internal flags on `PlanViewModel` (e.g. `outputDirectoryPathOverridden`, `planFileNameOverridden`) set to `true` only when the *user* changes the value via UI binding (not when the ViewModel writes the default itself).
2. Implement default-on-change for output directory: when `GenerationRootPath` setter runs and `outputDirectoryPathOverridden == false`, copy the new root to `GenerationOutputDirectoryPath` silently (using a private setter path that does not flip the override flag).
3. Initialize `PlanFileName` to `rename-plan.json` (sourced from `AppStrings.DefaultPlanFileName`) on construction; flip `planFileNameOverridden` on user write.
4. Expose `HasAdvancedOverrides` as a derived property; raise change notifications when either underlying flag changes.
5. Update `GenerateWorkspaceView.xaml` to wrap the override-only fields in a disclosure. Bind `IsVisible` of the inner stack to a local toggle (`IsAdvancedExpanded`) initialized from `HasAdvancedOverrides`. Show a compact summary line in collapsed state.
6. Add resource keys to `AppStrings.resx`; regenerate `AppStrings.Designer.cs`.
7. Update / add ViewModel tests covering the default + override behavior.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanViewModel"`
4. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `feat/`. (User-visible behavior change: defaulting fields and hiding inputs.)
2. Keep the PR scoped to ViewModel default logic, the Generate view disclosure, and the supporting resource keys + tests.
3. Title the PR with a Conventional Commit, for example `feat(ui): 360 plan file smart defaults`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- On a fresh launch, picking a photo folder no longer requires picking a save folder or typing a filename — the user can press the commit action immediately and a plan file is produced beside the photos.
- The output folder + filename + plan-path-preview block is hidden by default behind `Advanced ▾`.
- Expanding `Advanced ▾` and editing either value sticks: subsequent root-path changes do not overwrite the user-supplied output folder.
- Collapsed state shows a short helper line indicating where the plan will be saved.
- All new copy comes from `AppStrings.resx`.

## Tests
- New `src/Renamer.Tests/UI/PlanViewModelDefaultsTests.cs` (or extend `PlanViewModelTests.cs`):
  - Default plan filename is `rename-plan.json`.
  - Setting `GenerationRootPath` updates `GenerationOutputDirectoryPath` to match when not overridden.
  - User-supplied output directory is preserved across subsequent root-path changes.
  - `HasAdvancedOverrides` reflects override state.

## Test scope
- ViewModel default + override behavior. No filesystem IO required.

## Expected outputs
- `src/Renamer.UI/Plans/PlanViewModel.cs` — defaults + override flags + derived property.
- `src/Renamer.UI/Views/GenerateWorkspaceView.xaml` — `Advanced ▾` disclosure.
- `src/Renamer.UI/Resources/Strings/AppStrings.resx` (+ Designer) — new keys.
- `src/Renamer.Tests/UI/PlanViewModelDefaultsTests.cs` — new tests.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests pass locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- PR scope limited to this slice.
