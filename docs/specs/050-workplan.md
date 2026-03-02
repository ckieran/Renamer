# Workplan (Draft)

## Phase 0: Spec alignment
- Finalize product + requirements + acceptance + architecture.
- Confirm supported file types and naming format.

## Entry criteria before implementation
- Specs are approved through `080-cli-contract.md`.
- Slice docs `110` to `240` include concrete runbook sections:
  - implementation steps
  - commands to run
  - test scope
  - expected outputs

## Phase 1: Core hardening
- Execute slices in order:
  - `110-core-contract-models.md`
  - `120-serilog-cli-bootstrap.md`
  - `130-exif-reader-jpeg-nef.md`
  - `140-folder-date-range.md`
  - `150-name-generation.md`
  - `160-conflict-retry-policy.md`

## Phase 2: CLI wrapper
- Execute slices in order:
  - `170-plan-json-writer.md`
  - `180-apply-engine.md`
  - `190-report-json-writer.md`
  - `200-cli-plan-command.md`
  - `210-cli-apply-command.md`

## Phase 3: UI wrapper
- Execute slices in order:
  - `220-serilog-ui-bootstrap.md`
  - `230-maui-plan-view.md`
  - `240-maui-apply-flow.md`

## Phase 4: Polish
- Cross-platform validation on Windows/macOS.
- Documentation updates and retrospective cleanup.

## Delivery mode
- One slice per PR.
- Complete acceptance checks and tests for the current slice before starting the next slice.
- Keep each PR scoped to a single slice document.
