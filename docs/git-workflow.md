# Git Workflow

This document defines the repo's Git and GitHub issue workflow around slice pickup, implementation, and PR close-out.

## Slice pickup preflight
Complete these steps before editing code for a new slice:

1. Confirm the working tree is clean:
   - `git status --short`
2. Confirm the current branch is `main`:
   - `git branch --show-current`
3. Fast-forward local `main`:
   - `git pull --ff-only origin main`
4. Create the slice branch from `main` using the conventional commit type as prefix:
   - `git switch -c <type>/<slice-id>-<short-description>`
   - e.g. `git switch -c feat/150-name-generation` or `git switch -c refactor/290-string-resources`
   - See `docs/specs/070-engineering-contract.md` for the full type vocabulary and branch naming rules.
5. Confirm branch name matches the active slice:
   - `git branch --show-current`
6. If a matching GitHub issue exists for the slice:
   - move it to `In Progress` in the GitHub project
   - confirm the issue title and slice ID match the branch and spec

## Working rules
- One slice per branch.
- One branch per PR.
- Keep commits scoped to the active slice only.
- Do not mix unrelated cleanup or refactors into slice branches.
- All commit messages must follow Conventional Commits — see `docs/specs/070-engineering-contract.md`.

## Merge strategy

All PRs are merged via **squash merge**. The PR title becomes the single commit message on `main`, so it must be a valid Conventional Commit. This keeps `main` history linear and makes each slice atomically revertable.

## PR close-out
When opening a PR for a slice:

1. Set the PR title to a valid Conventional Commit using the slice ID as the subject:
   - Format: `<type>(<scope>): <slice-id> <short description>`
   - Examples: `feat(core): 150 name generation`, `refactor(ui): 290 extract string resources`
   - The type must match the branch prefix and the primary commit type used in the branch.
   - This title becomes the squash commit message on `main`.
2. Link the corresponding GitHub issue in the PR body:
   - prefer `Closes #<issue-number>` when the PR is intended to complete the slice
   - use `Refs #<issue-number>` only when the PR is partial or non-closing
3. Confirm the linked issue is in `In Progress` before requesting review.
4. Keep the PR scoped to the slice spec and acceptance checks only.
5. After merge:
   - update `docs/checklists/v1.md`
   - confirm the GitHub issue is closed by the PR merge

## Notes
- Specs remain the source of truth; GitHub issues and PRs are execution tracking artifacts.
- If no GitHub issue exists yet for a slice, continue with the documented Git preflight and create or backfill the issue separately.
