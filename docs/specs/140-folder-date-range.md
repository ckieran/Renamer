# 140 Folder Date Range

## Goal
Compute folder-level min/max capture date from photo metadata.

## In scope
- Range calculation from valid capture dates.
- Ignoring photos without capture date.

## Out of scope
- Destination naming.
- Conflict handling.

## Acceptance checks
- Mixed valid/missing dates produce correct min/max.
- All-missing folders are flagged as non-plannable.

## Tests
- Unit tests for min/max logic and empty valid-date set.

## Exit criteria
- Date range service logic passes tests.
