# CLI Contract (Draft)

## Scope
Defines current command-line behavior for machine-consumable planning and execution.

## Commands
- `renamer plan --root <path> --out <path>`
- `renamer apply --plan <path> --out <path>`

## Arguments
### `plan`
- Required:
  - `--root <path>`: root folder to scan.
  - `--out <path>`: output plan artifact path.

### `apply`
- Required:
  - `--plan <path>`: input plan artifact path.
  - `--out <path>`: output report artifact path.

## Validation order
### `plan`
1. Validate required args are present.
2. Validate `--root` exists and is a directory.
3. Validate parent directory for `--out` is writable.
4. Build and write plan artifact.

### `apply`
1. Validate required args are present.
2. Validate `--plan` exists and is readable.
3. Validate plan schema support (`schemaVersion`).
4. Validate parent directory for `--out` is writable.
5. Execute apply and write report artifact.

## Output contract
- Canonical output is JSON artifacts.
- Output files are overwritten if they already exist.
- Optional console text is informational only and not contractual.

## Failure behavior and exit codes
- `0`: success.
- `2`: argument or input validation failure.
- `3`: IO/path access failure.
- `4`: unsupported or invalid plan schema.
- `5`: conflict retry limit reached (execution abort).
- `6`: unexpected runtime error.
