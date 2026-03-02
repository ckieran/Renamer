# 230 MAUI Plan View

## Goal
Render plan JSON in a user-friendly preview list.

## In scope
- Select/load plan artifact.
- Display operations list and key summary values.
- Show loading/error states.

## Out of scope
- Executing apply.
- Editing plan contents.

## Acceptance checks
- Valid plan file renders operations and summary.
- Invalid/missing file shows actionable error.

## Tests
- ViewModel tests for load, map, and error states.

## Exit criteria
- Users can review planned operations in UI.

