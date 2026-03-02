# 240 MAUI Apply Flow

## Goal
Run apply from UI and present execution report results.

## In scope
- Trigger apply for selected plan artifact.
- Display progress and final report summary.
- Surface abort/failure outcomes clearly.

## Out of scope
- Advanced report analytics.
- Background scheduling.

## Acceptance checks
- Successful apply shows summary counts and drift info.
- Abort path (retry limit exceeded) is displayed with clear message.

## Tests
- ViewModel tests for success/failure/abort states.

## Exit criteria
- UI can execute and display report outcomes end-to-end.

