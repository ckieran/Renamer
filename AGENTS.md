## Project focus
- Spec-driven development with small, checkable deliverable chunks.
- Use the core library as the source of truth; build CLI and UI as wrappers.
- Prefer tests that exercise core logic via public APIs.

## Workflow expectations
- Open or update a spec document before starting a new feature.
- Each chunk should include acceptance criteria and a test plan.
- Keep changes scoped; avoid unrelated refactors.
- Use branches with prefix `codex/` when creating new branches.

## Testing
- Run unit tests for affected areas.
- Add or update tests alongside core changes.

## Docs
- Specs live under `docs/specs/`.
- Delivery checklists live under `docs/checklists/`.
