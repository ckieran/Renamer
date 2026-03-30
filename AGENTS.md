## Project focus
- Spec-driven development with small, checkable deliverable chunks.
- Prefer clear architecture boundaries and testable core logic.
- Optimize for short, reviewable PRs with minimal context overhead.

## Workflow expectations
- Create or update specs before implementing features.
- Each chunk should include acceptance criteria, test scope, and expected outputs.
- Keep changes scoped; avoid unrelated refactors.
- Use the conventional commit type as the branch prefix: `<type>/<slice-id>-<description>` (see `docs/specs/070-engineering-contract.md` for type vocabulary). Use `claude/` only for exploration or spec-only work.

## Deterministic slice start protocol
- Always start execution from `docs/checklists/v1.md`.
- Choose the first unchecked item in "Implementation slices (one PR per item)" as the active slice.
- Open the referenced slice spec under `docs/specs/` and use it as the implementation scope.
- Before any code edits, enforce the pre-implementation gate in `docs/specs/100-slice-template.md`.
- Do not implement if preflight fails; fix branch/preflight state first.
- Required preflight order:
  1. `git status --short` must be clean.
  2. `git branch --show-current` must be `main`.
  3. `git pull --ff-only origin main`.
  4. `git switch -c <type>/<slice-id>-<slice-name>` — choose the type from the conventional commits vocabulary in `docs/specs/070-engineering-contract.md`.
  5. Confirm branch name matches the active slice.
  6. If a matching GitHub issue exists, move it to `In Progress` and confirm it matches the active slice.
- Only after preflight passes: implement one slice, run slice-required commands/tests, open a PR linked to the corresponding issue, then update `docs/checklists/v1.md`.
- PR title must be a valid Conventional Commit: `<type>(<scope>): <slice-id> <short description>` — this becomes the squash commit message on `main`. See `docs/git-workflow.md` for examples.

## Error recovery

- If preflight fails at any step, stop immediately — do not proceed to implementation. Report current state (branch, dirty files, error) and wait for the user to resolve.
- If the standard test gate (`dotnet restore` / `build` / `test`) fails mid-slice, stop and fix before opening a PR. Do not open a PR with failing tests.
- Do not attempt automatic cleanup of uncommitted changes or branch state. Destructive git operations (reset, checkout ., clean) require explicit user instruction.

## Required context pack (read before implementing a slice)
- Mandatory baseline for every slice:
  - `docs/checklists/v1.md`
  - `docs/conventions.md`
  - `docs/git-workflow.md`
  - `docs/specs/040-architecture.md`
  - `docs/specs/050-workplan.md`
  - `docs/specs/060-plan-schema.md`
  - `docs/specs/070-engineering-contract.md`
  - `docs/specs/100-slice-template.md`
  - Active slice spec (`docs/specs/<slice>.md`)
- Conditional reads by slice type:
  - CLI slices: read `docs/specs/080-cli-contract.md`.
  - UI slices: read relevant UI specs/contracts (`220+` and dependencies).
  - Contract/report/plan serialization slices: re-check `060-plan-schema.md` immediately before implementation.
- Dependency rule:
  - If the active slice depends on outputs from earlier slices, read those prior slice specs before editing.

## Spec orchestrator (reusable fast planning mode)
- Default to `spec_only` for project restarts or major pivots.
- In `spec_only`, do not edit `src/` until the spec gate is complete.
- Canonical planning artifacts live under `docs/specs/` and `docs/checklists/`.
- Treat legacy planning files outside canonical paths as historical context.

### Spec gate (must complete before code changes)
- Resolve and record these decisions up front:
  1. Logging policy
  2. Tech stack
  3. Conflict policy
  4. Data contract(s) between planning and execution phases
  5. Output contract(s) for machine vs human consumers
- Ensure these planning artifacts exist and are aligned:
  - Spec index
  - Requirements/spec baseline
  - Architecture baseline
  - Workplan
  - Engineering contract (commands, test gates, PR rules)
  - Slice template
  - Delivery checklist
- Generate atomic slices and order them for one-slice-per-PR execution.

### Slice quality standard
- Each slice must contain:
  - Goal
  - In scope
  - Out of scope
  - Implementation steps
  - Commands to run
  - Acceptance checks
  - Tests and minimum passing scope
  - Expected outputs
  - Definition of Done
- Keep slices sized for a single concise PR and review pass.

### Build mode (after spec gate)
- Implement exactly one slice per PR.
- Run slice-specific commands plus standard restore/build/test gates.
- Update the delivery checklist when a slice is completed.

## Testing
- Run unit tests for affected areas.
- Add or update tests alongside core changes.
- Prefer fast unit/integration tests early; add broader end-to-end tests later.

## Docs
- Specs live under `docs/specs/`.
- Delivery checklists live under `docs/checklists/`.
- Implementation conventions live under `docs/conventions.md`.
- Git and GitHub workflow guidance lives under `docs/git-workflow.md`.
