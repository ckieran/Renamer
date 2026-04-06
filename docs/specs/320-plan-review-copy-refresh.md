# Plan Review Copy Refresh

## Slice ID
`320-plan-review-copy-refresh`

## Goal
Rewrite the middle workflow area so users understand they are safely reviewing the plan before any rename happens.

## In scope
- Update plan review resource values in `AppStrings.resx`, including:
  - `PreviewHeading`
  - `PreviewDescription`
  - `PreviewSectionHeader`
  - `PreviewInstructions`
  - `PreviewPlaceholder`
  - `PreviewBrowse`
  - `PreviewButtonLoad`
  - `PreviewIdle*`
  - `PreviewLoading*`
  - `PreviewErrorHeading`
  - `PreviewSummary*`
  - `PreviewField*`
  - `PreviewOperations*`
  - `PreviewStatus*`
- Update preview-specific fallback text assembled in `PlanViewModel.cs`, including the per-operation summary string currently built as `"{count} files, {count} missing EXIF"`.
- Allow user-facing text to shift from `Preview` to a friendlier `Review` or `Check` framing if that improves clarity, while keeping internal type and file names unchanged.

## Out of scope
- Shared shell strings covered by `300-ui-copy-baseline.md`.
- Generate strings.
- Apply strings.
- Plan-loading behavior, summary data shape, or list layout changes.

## Dependency notes
- Depends on `290-string-resources.md`.
- Should follow the shared voice rules from `300-ui-copy-baseline.md`.
- Must not rename internal classes, enums, or files just to match user-facing copy.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c refactor/320-plan-review-copy-refresh`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `refactor/320-plan-review-copy-refresh`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Audit all `Preview*` keys and current preview-state messages.
2. Reframe the area as a safe review step, emphasizing that users are checking proposed renames before running anything.
3. Shorten summary and operations section copy so labels stay clear without sounding like internal system terminology.
4. Move any remaining preview-facing raw string assembly in `PlanViewModel.cs` to resource-backed wording if needed for consistency.
5. Update tests that assert preview-specific copy.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanViewModel|FullyQualifiedName~Stepper"`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `refactor/`.
2. Keep the PR scoped to plan review copy and directly related tests only.
3. Title the PR with a Conventional Commit, for example `refactor(ui): 320 simplify plan review copy`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- The middle step clearly reads as a safe review/check step before rename execution.
- Load, idle, loading, summary, and error copy are easier to scan and less technical than the current wording.
- Users can understand summary cards and operation rows without needing to interpret internal terms such as `operations` or `EXIF` unless the detail is truly necessary.
- Preview-specific copy stays precise enough for support and debugging without becoming verbose.

## Tests
- Update `src/Renamer.Tests/UI/PlanViewModelTests.cs`.
- Update `src/Renamer.Tests/UI/StepperShellViewModelTests.cs` only if the chosen user-facing step wording changes and is not already covered by slice `300`.

## Test scope
- Preview-state text, plan review labels, and resource-backed fallback text.

## Expected outputs
- Updated preview-context entries in `src/Renamer.UI/Resources/Strings/AppStrings.resx`
- Regenerated `src/Renamer.UI/Resources/Strings/AppStrings.Designer.cs`
- Preview-related `PlanViewModel` text updated where raw strings still exist
- Preview-related UI tests aligned to the new wording

## Exit criteria
- The plan review area explains the check-before-you-run step in plain language.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are updated and pass locally.
- Checklist item for this slice is updated.
- PR scope is limited to plan review copy and related tests.
