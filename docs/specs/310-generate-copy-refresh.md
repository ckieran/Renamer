# Generate Copy Refresh

## Slice ID
`310-generate-copy-refresh`

## Goal
Rewrite the Generate area so it sounds simple and reassuring, with concise guidance about choosing folders and making a plan.

## In scope
- Update generate-context resource values in `AppStrings.resx`, including:
  - `GenerateHeading`
  - `GenerateDescription`
  - `GenerateSectionHeader`
  - `GenerateInstructions`
  - `GenerateLabel*`
  - `GeneratePlaceholder*`
  - `GenerateBrowse*`
  - `GenerateButton*`
  - `GenerateStatus*`
  - `GenerateError*`
  - `GenerateFolderPicker*`
- Keep the required filename `rename-plan.json` visible where it materially helps the user, but avoid overusing internal terms like `artifact`.
- Preserve current generate behavior and validation rules; this slice only changes wording.

## Out of scope
- Shared shell strings covered by `300-ui-copy-baseline.md`.
- Preview/plan review strings.
- Apply strings.
- Any change to generation logic, validation thresholds, or file output behavior.

## Dependency notes
- Depends on `290-string-resources.md`.
- Should follow the shared voice rules from `300-ui-copy-baseline.md`.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c refactor/310-generate-copy-refresh`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `refactor/310-generate-copy-refresh`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Audit all `Generate*` keys and group them by purpose: setup guidance, field labels, action labels, status updates, and recovery messages.
2. Replace technical wording with everyday language that answers:
   - what this step does
   - what the user needs to choose
   - what happens next
3. Keep action labels short and scannable, especially browse and submit buttons.
4. Rewrite validation and error copy so each message points to a single fix instead of describing internals.
5. Update tests that assert generate-specific strings or messages.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~PlanGeneration|FullyQualifiedName~PlanViewModel"`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `refactor/`.
2. Keep the PR scoped to generate-area copy and directly related tests only.
3. Title the PR with a Conventional Commit, for example `refactor(ui): 310 simplify generate area copy`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- A new user can understand the Generate step without knowing what a `root folder`, `artifact`, or `plan generation` means internally.
- Field labels, helper text, and button labels are shorter and more conversational than the current copy.
- Success, validation, and failure messages explain the next action plainly.
- No generate-area string becomes vague; the copy remains specific enough to complete the task correctly.

## Tests
- Update `src/Renamer.Tests/UI/PlanGenerationViewModelTests.cs`.
- Update any `src/Renamer.Tests/UI/PlanViewModelTests.cs` assertions that cover generate-state copy.

## Test scope
- Generate-state messages, validation messaging, and button/label bindings.

## Expected outputs
- Updated generate-context entries in `src/Renamer.UI/Resources/Strings/AppStrings.resx`
- Regenerated `src/Renamer.UI/Resources/Strings/AppStrings.Designer.cs`
- Generate-related UI tests aligned to the new wording

## Exit criteria
- The Generate area feels calm, clear, and action-oriented without changing how generation works.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are updated and pass locally.
- Checklist item for this slice is updated.
- PR scope is limited to generate-copy changes and related tests.
