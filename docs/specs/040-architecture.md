# Architecture Spec (Draft)

## Project layout (target)
- `Renamer.Core` — core logic (EXIF, range, rename planning).
- `Renamer.Cli` — console wrapper for core, testable in isolation.
- `Renamer.UI` — MAUI UI wrapper over core.
- `Renamer.Tests` — unit tests for core + minimal CLI tests.

## Boundaries
- `Renamer.Core` has no UI dependencies.
- `Renamer.Cli` uses core services and writes to stdout/stderr.
- `Renamer.UI` uses core services via DI.

## Key interfaces
- EXIF extraction and file system access remain in core.
- Rename plan generation is pure and deterministic where possible.

## Cross-platform considerations
- Use `Path` APIs and normalize separators.
- Avoid OS-specific path invalid characters when generating new names.
