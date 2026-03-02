# Plan/Report Schema Spec (Draft)

## Scope
Defines the v1 JSON contract between planning and execution for CLI and UI wrappers.

## Files
- Plan artifact: `rename-plan.v1.json`
- Execution report artifact: `rename-report.v1.json`

## Compatibility and versioning
- `schemaVersion` is required in both files.
- v1 uses `schemaVersion: "1.0"`.
- Any breaking field change requires a new schema version.

## Plan schema (`rename-plan.v1.json`)

### Required top-level fields
- `schemaVersion` (string)
- `planId` (string, UUID recommended)
- `createdAtUtc` (string, ISO-8601 UTC)
- `rootPath` (string, absolute path)
- `operations` (array of operation objects)
- `summary` (object)

### Operation object (required fields)
- `opId` (string, UUID recommended)
- `sourcePath` (string, absolute path)
- `plannedDestinationPath` (string, absolute path)
- `reason` (object)

### `reason` object
- `startDate` (string, `YYYY-MM-DD`)
- `endDate` (string, `YYYY-MM-DD`)
- `filesConsidered` (integer, >= 0)
- `filesSkippedMissingExif` (integer, >= 0)

### `summary` object
- `operationCount` (integer, >= 0)
- `warnings` (integer, >= 0)

### Plan invariants
- `operations.length == summary.operationCount`
- `plannedDestinationPath` values must be unique within a plan.
- Naming format:
  - Range: `YYYY-MM-DD - YYYY-MM-DD - <rest of name>`
  - Single day: `YYYY-MM-DD - <rest of name>`
- `<rest of name>` is the original source folder name unchanged.

## Report schema (`rename-report.v1.json`)

### Required top-level fields
- `schemaVersion` (string)
- `planId` (string)
- `startedAtUtc` (string, ISO-8601 UTC)
- `finishedAtUtc` (string, ISO-8601 UTC)
- `results` (array of result objects)
- `summary` (object)

### Result object (required fields)
- `opId` (string)
- `sourcePath` (string)
- `plannedDestinationPath` (string)
- `actualDestinationPath` (string or null)
- `status` (string enum: `success`, `failed`, `skipped`)
- `attempts` (integer, >= 1)
- `warnings` (array of strings)
- `error` (string or null)

### `summary` object
- `success` (integer, >= 0)
- `failed` (integer, >= 0)
- `skipped` (integer, >= 0)
- `drifted` (integer, >= 0)

### Report invariants
- `summary.success + summary.failed + summary.skipped == results.length`
- `drifted` counts entries where:
  - `status == "success"` and
  - `plannedDestinationPath != actualDestinationPath`

## Execution semantics (adaptive-only)
- Executor attempts rename to `plannedDestinationPath` first.
- On destination conflict, executor retries with deterministic numeric suffixes (` (1)`, ` (2)`, ...).
- Maximum retry attempts: 10 suffix retries after the initial attempt.
- `attempts` includes the initial attempt.
- When final destination differs from planned destination, report it and increment `drifted`.
- If destination remains unresolved after max retries, executor aborts current plan execution and records failure.
- Missing/invalid EXIF handling is a planning concern: file is skipped and warning is logged.

## Validation rules
- Fail plan load when required fields are missing or type-invalid.
- Fail apply when `schemaVersion` unsupported.
- Fail apply when `planId` is missing.
- Fail apply when an operation has empty `sourcePath` or `plannedDestinationPath`.

## CLI output contract (v1)
- CLI writes canonical output as JSON artifacts only.
- Human-readable console output is optional and not required by contract.

## Example plan
```json
{
  "schemaVersion": "1.0",
  "planId": "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
  "createdAtUtc": "2026-03-01T16:10:00Z",
  "rootPath": "/photos",
  "operations": [
    {
      "opId": "7c730a84-4b07-4f56-8758-9906cf488e6b",
      "sourcePath": "/photos/Trip A",
      "plannedDestinationPath": "/photos/2024-06-12 - 2024-06-14 - Trip A",
      "reason": {
        "startDate": "2024-06-12",
        "endDate": "2024-06-14",
        "filesConsidered": 120,
        "filesSkippedMissingExif": 3
      }
    }
  ],
  "summary": {
    "operationCount": 1,
    "warnings": 3
  }
}
```

## Example report
```json
{
  "schemaVersion": "1.0",
  "planId": "d609111f-4fbb-4de3-8d6c-faf102a6fdb0",
  "startedAtUtc": "2026-03-01T16:11:00Z",
  "finishedAtUtc": "2026-03-01T16:11:01Z",
  "results": [
    {
      "opId": "7c730a84-4b07-4f56-8758-9906cf488e6b",
      "sourcePath": "/photos/Trip A",
      "plannedDestinationPath": "/photos/2024-06-12 - 2024-06-14 - Trip A",
      "actualDestinationPath": "/photos/2024-06-12 - 2024-06-14 - Trip A (1)",
      "status": "success",
      "attempts": 2,
      "warnings": [
        "Destination conflict; applied suffix (1)."
      ],
      "error": null
    }
  ],
  "summary": {
    "success": 1,
    "failed": 0,
    "skipped": 0,
    "drifted": 1
  }
}
```
