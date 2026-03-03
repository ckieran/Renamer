# Slice Template

## Slice ID
`XXX-name`

## Goal
One-sentence outcome.

## In scope
- Concrete behavior to implement.

## Out of scope
- Explicit non-goals for this slice.

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
1. Branch from current `main` using prefix `codex/` (for example `codex/xxx-slice-name`).
2. Keep one slice per branch and one branch per PR.
3. Commit only files related to this slice.
4. Open PR back into `main` with slice ID in title/body.

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

## Definition of Done
- Acceptance checks in this slice are satisfied.
- Tests listed in this slice are implemented and pass locally.
- Checklist item for this slice is updated.
- Branch follows `codex/` naming and is ready for PR into `main`.
- PR scope is limited to this slice (no unrelated refactors).
