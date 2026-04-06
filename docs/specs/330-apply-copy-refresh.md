# Apply Copy Refresh

## Slice ID
`330-apply-copy-refresh`

## Goal
Rewrite the Apply area so execution, outcomes, warnings, and failures feel direct and understandable instead of technical.

## In scope
- Update apply-context resource values in `AppStrings.resx`, including:
  - `ApplyHeading`
  - `ApplyDescription`
  - `ApplyPlanArtifact*`
  - `ApplyButton*`
  - `ApplySectionHeader`
  - `ApplyInstructions`
  - `ApplySummary*`
  - `ApplyField*`
  - `ApplyStatus*`
  - `ApplyErrorHeading`
  - `ApplyErrorTitle*`
  - `ApplyErrorMessage*`
- Update apply-result fallback copy still assembled in `PlanViewModel.cs`, including:
  - `Not moved`
  - `No warnings`
  - `No error`
- Keep the distinction between review/load messaging and actual rename execution clear.

## Out of scope
- Shared shell strings covered by `300-ui-copy-baseline.md`.
- Generate strings.
- Plan review strings.
- Apply engine behavior, retry policy, report schema, or result list structure.

## Dependency notes
- Depends on `290-string-resources.md`.
- Should follow the shared voice rules from `300-ui-copy-baseline.md`.
- Must preserve enough specificity that retry-limit, invalid-plan, and file-system failures remain distinct to the user.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` with required prefix:
   - `git switch -c refactor/330-apply-copy-refresh`
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `refactor/330-apply-copy-refresh`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Audit all `Apply*` keys and the remaining raw apply-facing strings assembled in `PlanViewModel.cs`.
2. Rewrite the step intro and instructions so users understand this is the point where renaming actually runs.
3. Simplify summary field labels and result-row fallback text so status, warnings, and errors read naturally.
4. Rewrite failure titles and failure messages to be short, plain, and actionable while preserving the distinct failure classes already mapped in code.
5. Update tests that assert apply-specific copy.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test src/Renamer.Tests/Renamer.Tests.csproj --filter "FullyQualifiedName~ApplyFlow|FullyQualifiedName~PlanViewModel"`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `refactor/`.
2. Keep the PR scoped to apply-area copy and directly related tests only.
3. Title the PR with a Conventional Commit, for example `refactor(ui): 330 simplify apply area copy`.
4. Link the matching issue with `Closes #<issue-number>` when the slice is complete.

## Acceptance checks
- The Apply step clearly signals that this is the live rename action, not another preview.
- Summary labels, result labels, warnings, and empty/fallback text are shorter and less technical than the current wording.
- Error copy explains what happened and what to do next without exposing internal implementation language.
- Distinct failure states still read differently enough that users can tell validation, invalid plan, and file-system problems apart.

## Tests
- Update `src/Renamer.Tests/UI/ApplyFlowViewModelTests.cs`.
- Update any `src/Renamer.Tests/UI/PlanViewModelTests.cs` assertions that cover apply-state text or fallback strings.

## Test scope
- Apply-state messages, error mapping copy, summary/result labels, and fallback text.

## Expected outputs
- Updated apply-context entries in `src/Renamer.UI/Resources/Strings/AppStrings.resx`
- Regenerated `src/Renamer.UI/Resources/Strings/AppStrings.Designer.cs`
- Apply-related `PlanViewModel` text updated where raw strings still exist
- Apply-related UI tests aligned to the new wording

## Exit criteria
- The Apply area communicates execution and outcomes in clear, low-jargon language.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are updated and pass locally.
- Checklist item for this slice is updated.
- PR scope is limited to apply-copy changes and related tests.
