# Slice Template

## Slice ID
`XXX-name`

## Goal
One-sentence outcome.

## In scope
- Concrete behavior to implement.

## Out of scope
- Explicit non-goals for this slice.

## Pre-implementation gate (must pass before code edits)
1. Verify working tree is clean:
   - `git status --short` returns no changes.
2. Verify current branch is `main`:
   - `git branch --show-current` returns `main`.
3. Update local `main` from origin:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` using the conventional commit type as prefix:
   - `git switch -c <type>/XXX-name`
   - Choose the type that matches this slice: `feat`, `fix`, `refactor`, `docs`, `test`, `build`, `ci`, or `chore` — see `docs/specs/070-engineering-contract.md` for the full vocabulary.
5. Confirm branch naming matches this slice ID:
   - `git branch --show-current` equals `<type>/XXX-name`.
6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches this slice ID.
7. Do not edit code until steps 1-6 are complete.

## Implementation steps
1. Step 1.
2. Step 2.
3. Step 3.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

## Git/PR workflow
0. Setup - change to `main` branch and pull latest from origin to prepare for new work.
1. Branch from current `main` using the conventional commit type as prefix (for example `feat/xxx-slice-name` or `refactor/xxx-slice-name`). See `docs/specs/070-engineering-contract.md` for type vocabulary and branch naming rules.
2. Keep one slice per branch and one branch per PR.
3. Commit only files related to this slice.
4. If a matching GitHub issue exists, ensure it is `In Progress` before opening the PR.
5. Open PR back into `main` with slice ID in title/body.
6. Link the corresponding issue in the PR body:
   - use `Closes #<issue-number>` when the PR completes the slice
   - use `Refs #<issue-number>` only for partial work
7. Include the slice ID in PR title and body.

## Acceptance checks
- Observable check 1.
- Observable check 2.

## Tests
- Unit/integration tests to add or update.

## Test scope
- Minimum tests that must pass for this slice.

## Expected outputs
- Files/artifacts created or updated by this slice.

## Exit criteria
- PR merged with passing tests for this slice only.
- PR links the corresponding GitHub issue when one exists.

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are implemented and pass locally.
- Checklist item for this slice is updated.
- Branch pre-implementation gate completed before first code edit.
- Branch name follows `<type>/` (conventional commit type) and maps to this slice ID.
- PR scope is limited to this slice (no unrelated refactors).
- If a matching GitHub issue exists, it is linked from the PR and moved to `In Progress` when the slice is picked up.
