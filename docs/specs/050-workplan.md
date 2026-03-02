# Workplan (Draft)

## Phase 0: Spec alignment
- Finalize product + requirements + acceptance + architecture.
- Confirm supported file types and naming format.

## Phase 1: Core hardening
- Ensure EXIF extraction + date range logic is stable.
- Add tests for Nikon `.nef` and JPEG handling.
- Add tests for missing EXIF behavior (skip file + warning log).
- Configure Serilog from the start with console sink and one long-lived file sink per executable (`renamer.log`), including info/error operational records.

## Phase 2: CLI wrapper
- Create `Renamer.Cli` project.
- Implement `plan` and `apply` modes.
- Implement JSON artifact contract:
  - `rename-plan.v1.json` from `plan`
  - `rename-report.v1.json` from `apply`
- Keep execution mode adaptive-only for v1.
- Keep operations rename-only for v1 (no operation type expansion yet).
- Implement and verify deterministic conflict auto-suffix behavior in preview and apply with max 10 retries.
- Abort current plan execution when collision retries exceed max.
- Add minimal integration tests or scripted fixture runs.

## Phase 3: UI wrapper
- Wire MAUI UI to core + reuse CLI flow concepts.
- Implement preview list + apply confirmation.
- Improve error surfaces.

## Phase 4: Polish
- Extend logging/audit detail as needed based on production usage.
- Cross-platform validation on Windows/macOS.
- Documentation updates.
