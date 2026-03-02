## Project focus
- Spec-driven development with small, checkable deliverable chunks.
- Prefer clear architecture boundaries and testable core logic.
- Optimize for short, reviewable PRs with minimal context overhead.

## Workflow expectations
- Create or update specs before implementing features.
- Each chunk should include acceptance criteria, test scope, and expected outputs.
- Keep changes scoped; avoid unrelated refactors.
- Use branches with prefix `codex/` when creating new branches.

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
