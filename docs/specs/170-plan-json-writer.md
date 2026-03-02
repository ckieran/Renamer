# 170 Plan JSON Writer

## Goal
Persist `rename-plan.v1.json` from core plan models.

## In scope
- Write canonical plan artifact JSON.
- Populate required metadata and summary fields.

## Out of scope
- CLI argument parsing.
- Report generation.

## Acceptance checks
- Generated file validates against required field set.
- Operation count matches summary invariant.

## Tests
- Integration-style unit test writing plan JSON to temp path.

## Exit criteria
- Plan writer produces valid v1 contract output.
