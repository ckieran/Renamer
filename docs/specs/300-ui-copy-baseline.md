# Shared UI Copy Baseline

## Slice ID
`300-ui-copy-baseline`

## Goal
Define and apply a plain-language copy baseline for the shared shell so every workflow area starts from the same calm, simple tone.

## In scope
- Establish the shared voice rules for UI copy:
  - plain language over internal terminology
  - short, direct sentences
  - explanatory without sounding technical
  - one clear next step in validation and error copy
- Update shared shell strings in `AppStrings.resx`:
  - `MainPage*`
  - `WorkflowRail*`
  - `StepGenerate*`
  - `StepPreview*`
  - `StepApply*`
- Extract workflow-step badge labels from `PlanWorkflowStepItem.cs` into `AppStrings.resx` so `done`, `error`, and `needs info` can be rewritten consistently.
- Keep internal type names such as `PreviewPlan` and `ApplyPlan` unchanged; this slice is user-facing text only.

## Out of scope
- Generate workspace field, helper, status, and error copy.
- Plan review workspace field, helper, status, and error copy.
- Apply workspace field, helper, status, and error copy.
- Layout, navigation, styling, or behavior changes.
- CLI text changes.

## Dependency notes
- This slice depends on `290-string-resources.md` so copy updates are resource-driven rather than scattered across XAML and ViewModel code.
- Later copy-refresh slices must follow the voice rules defined here instead of redefining tone per area.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c refactor/300-ui-copy-baseline`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `refactor/300-ui-copy-baseline`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Add shared copy principles to the top of the implementation PR description or issue so reviewers can judge wording consistently.
2. Update shared shell resource values to remove internal jargon such as `workflow`, `artifact`, or other implementation-first wording where a simpler phrase works.
3. Add resource keys for workflow-step badge labels and replace the hardcoded text in `PlanWorkflowStepItem.cs`.
4. Keep button and label naming stable enough that later generate/plan/apply slices can reuse the same tone without another shell pass.
5. Update shell-focused tests that assert step titles, descriptions, or badge labels.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~Stepper|FullyQualifiedName~PlanViewModel"`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `refactor/`.
2. Keep the PR scoped to shared-shell copy only.
3. Title the PR with a Conventional Commit, for example `refactor(ui): 300 define shared ui copy baseline`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- The top-level page header and rail description read naturally to a non-technical user on first launch.
- Step titles and step descriptions are shorter and less implementation-focused than the current wording.
- Workflow badge text is no longer hardcoded in `PlanWorkflowStepItem.cs`.
- Shared copy uses one consistent tone across the shell: calm, concise, and explanatory.

## Tests
- Update `src/Renamer.Tests/UI/StepperShellViewModelTests.cs` for shared step title/description/badge expectations.
- Update any `PlanViewModel` tests that assert shared shell strings.

## Test scope
- Shell-level copy bindings and step-status text only.

## Expected outputs
- `src/Renamer.UI/Resources/Strings/AppStrings.resx`
- `src/Renamer.UI/Resources/Strings/AppStrings.Designer.cs`
- `src/Renamer.UI/Plans/PlanWorkflowStepItem.cs`
- Shared-shell UI tests updated for the new wording

## Exit criteria
- Shared shell text sets the plain-language baseline for the rest of the UI copy refresh.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are updated and pass locally.
- Checklist item for this slice is updated.
- PR scope is limited to shared-shell copy and related tests.
