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
- Planner must emit a JSON plan artifact for execution (`rename-plan.json`).
- Executor must consume the JSON plan artifact and emit a JSON report artifact (`rename-report.json`).

## Non-functional requirements
- Cross-platform: Windows and macOS.
- Desktop-only in v1: no iOS, iPadOS, Android, or other mobile/tablet targets.
- Safe by default: no changes unless user confirms.
- Errors are actionable: show message and keep log.
- Core logic testable without UI.

## Constraints and assumptions
- Only local filesystem paths.
- Initial support limited to `.nef`, `.jpg`, `.jpeg`.
- UI uses .NET MAUI desktop targets only (Windows + Mac Catalyst).
- Execution mode is adaptive-only in v1.
- Folder rename is the only operation in v1.

## Naming and conflict policy
- Primary naming format is date-first:
  - Range: `YYYY-MM-DD - YYYY-MM-DD - <rest of name>`
  - Single day: `YYYY-MM-DD - <rest of name>`
- `<rest of name>` is the original current folder name unchanged (for example `Rome weekend`).
- Conflict resolution appends numeric suffixes to the destination folder name until a unique path is found, with a maximum of 10 retries.
- If a unique destination is not found after 10 retries, abort current plan execution and mark it as failed in the report.

## Output artifact policy
- `plan` and `apply` overwrite output files if they already exist.
- Default artifact names remain `rename-plan.json` and `rename-report.json`.

## Logging policy
- Operational logs are written to one long-lived file per executable.
- Default path roots:
  - Windows: `%LOCALAPPDATA%/Renamer/logs/<executable>.log`
  - macOS: `~/Library/Logs/Renamer/<executable>.log`
- Log path is provided by OS-aware constants/provider, not hardcoded call sites.
- Logs should include info through error records suitable for debugging.
- Per-run outcomes are recorded in execution report JSON (`rename-report.json`), not via per-run log files.

## Exit code policy
- `0`: success.
- `2`: invalid arguments or input validation failure.
- `3`: IO/path access failure.
- `4`: unsupported/invalid plan schema.
- `5`: conflict retry limit reached (plan execution abort).
- `6`: unexpected runtime error.

## CLI output mode
- Canonical machine output is JSON artifacts.
- Human-readable console output is optional and non-contractual.

## Plan and report contract
- Plan file: `rename-plan.json`.
- Report file: `rename-report.json`.
- Required plan metadata: `schemaVersion`, `planId`, `createdAtUtc`, `rootPath`.
- Each planned operation includes: `opId`, `sourcePath`, `plannedDestinationPath`, `reason`.
- Report entries include: `opId`, `sourcePath`, `plannedDestinationPath`, `actualDestinationPath`, `status`, `attempts`, `warnings`, `error`.
- Execution tries `plannedDestinationPath` first and compensates with suffix retries on collision.
