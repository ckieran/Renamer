# Acceptance Criteria (Draft)

Each deliverable chunk is considered complete when its acceptance checks pass.

## Chunk A: Core date extraction + range
Acceptance:
- Given a folder with JPEG/NEF files and EXIF dates, the min/max dates are computed correctly.
- Files with missing or invalid EXIF are skipped, do not crash the scan, and produce warning log entries.
- Unit tests cover happy path + missing metadata.

## Chunk B: Rename plan generation
Acceptance:
- Given a folder tree, a deterministic rename plan is produced.
- Destination conflicts are resolved by deterministic auto-suffixing so planned destination paths are unique.
- Date formatting follows spec (single date vs range).
- `<rest of name>` is the source folder name unchanged in generated destination names.
- Unit tests cover naming edge cases.
- Unit tests cover conflict suffix assignment order.

## Chunk C: Console wrapper (CLI)
Acceptance:
- CLI accepts a root path and writes preview output as JSON only.
- CLI writes plan output to `rename-plan.v1.json`.
- CLI apply consumes `rename-plan.v1.json` and writes `rename-report.v1.json`.
- CLI exits non-zero on errors.
- Basic integration test or scripted sample run exists.
- Adaptive collision handling is verified with max 10 retries; if unresolved, plan execution aborts and report captures failure.
- Report entries include `sourcePath` for each operation result.

## Chunk D: MAUI UI (preview + apply)
Acceptance:
- User can select a folder.
- User can view a list of planned operations.
- User can apply the plan and see results.
- Errors surfaced in UI and recorded in log.

## Chunk E: Logging + audit
Acceptance:
- All operations are logged with timestamps and status.
- Each executable writes to one long-lived log file (no per-run log splitting required).
