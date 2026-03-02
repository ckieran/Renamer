# 190 Report JSON Writer

## Goal
Persist `rename-report.v1.json` with full operation outcomes.

## In scope
- Write required top-level report fields.
- Include `sourcePath`, planned/actual destination, status, attempts, warnings, error.
- Compute summary counters including `drifted`.

## Out of scope
- UI rendering.
- Log formatting.

## Acceptance checks
- Report includes required fields for each result.
- Summary invariants hold.

## Tests
- Unit tests for summary math and drift counting.

## Exit criteria
- Report writer produces valid v1 contract output.
