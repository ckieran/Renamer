# 150 Name Generation

## Goal
Generate folder destination names using date-first format and unchanged original folder name.

## In scope
- Multi-day format: `YYYY-MM-DD - YYYY-MM-DD - <rest of name>`.
- Single-day format: `YYYY-MM-DD - <rest of name>`.
- `<rest of name>` equals current folder name unchanged.

## Out of scope
- Conflict suffixing.
- Apply execution.

## Acceptance checks
- Single-day and multi-day examples format correctly.
- Original folder text is preserved verbatim.

## Tests
- Unit tests for representative naming inputs.

## Exit criteria
- Naming function is deterministic and tested.
