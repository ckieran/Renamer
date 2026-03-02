# 110 Core Contract Models

## Goal
Define v1 plan/report DTOs in `Renamer.Core` that match schema spec.

## In scope
- Add core model types for plan and report JSON artifacts.
- Include required fields from `060-plan-schema.md`.

## Out of scope
- File IO serialization.
- CLI/UI wiring.

## Acceptance checks
- Models represent all required fields for plan/report.
- Models support nullable/error fields as specified.

## Tests
- Serialization shape test with in-memory JSON (no disk IO).

## Exit criteria
- Core DTOs compile and tests pass.

