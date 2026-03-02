# Requirements Spec (Draft)

## Functional requirements
- Generate a rename plan from a selected root folder.
- Extract EXIF capture dates for supported file types.
- Compute date range per folder (min/max capture date).
- Format new folder names as:
  - `YYYY-MM-DD - YYYY-MM-DD - <rest of name>` when min != max
  - `YYYY-MM-DD - <rest of name>` when min == max
- For files with missing/invalid EXIF capture date: skip file and emit a warning log entry.
- For folder destination name conflicts: auto-suffix destination name deterministically (for example ` (1)`, ` (2)`).
- Preview is produced as JSON only (no required console rendering in CLI).
- Execute renames with explicit confirmation.
- Log all operations with status (success/failure/skip).
- Planner must emit a JSON plan artifact for execution (`rename-plan.v1.json`).
- Executor must consume the JSON plan artifact and emit a JSON report artifact (`rename-report.v1.json`).

## Non-functional requirements
- Cross-platform: Windows and macOS.
- Safe by default: no changes unless user confirms.
- Errors are actionable: show message and keep log.
- Core logic testable without UI.

## Constraints and assumptions
- Only local filesystem paths.
- Initial support limited to `.nef`, `.jpg`, `.jpeg`.
- UI uses .NET MAUI.
- Execution mode is adaptive-only in v1.
- Folder rename is the only operation in v1.

## Naming and conflict policy
- Primary naming format is date-first:
  - Range: `YYYY-MM-DD - YYYY-MM-DD - <rest of name>`
  - Single day: `YYYY-MM-DD - <rest of name>`
- `<rest of name>` is the original current folder name unchanged (for example `Rome weekend`).
- Conflict resolution appends numeric suffixes to the destination folder name until a unique path is found, with a maximum of 10 retries.
- If a unique destination is not found after 10 retries, abort current plan execution and mark it as failed in the report.

## Logging policy
- Operational logs are written to one long-lived file per executable (for example CLI and UI each maintain their own file).
- Logs should include info through error records suitable for debugging.
- Per-run outcomes are recorded in execution report JSON (`rename-report.v1.json`), not via per-run log files.

## Plan and report contract (v1)
- Plan file: `rename-plan.v1.json`.
- Report file: `rename-report.v1.json`.
- Required plan metadata: `schemaVersion`, `planId`, `createdAtUtc`, `rootPath`.
- Each planned operation includes: `opId`, `sourcePath`, `plannedDestinationPath`, `reason`.
- Report entries include: `opId`, `sourcePath`, `plannedDestinationPath`, `actualDestinationPath`, `status`, `attempts`, `warnings`, `error`.
- Execution tries `plannedDestinationPath` first and compensates with suffix retries on collision.
