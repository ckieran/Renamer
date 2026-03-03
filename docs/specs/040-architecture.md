# Architecture Spec (Draft)

## Project layout (target)
- `Renamer.Core` - core logic (EXIF, range, rename planning, apply engine, serializers).
- `Renamer.Cli` - console wrapper for core, testable in isolation.
- `Renamer.UI` - MAUI desktop UI wrapper over core (Windows + Mac Catalyst).
- `Renamer.Tests` - unit tests for core + CLI integration tests + UI ViewModel tests.

## Boundaries
- `Renamer.Core` has no UI dependencies.
- `Renamer.Cli` composes core services and writes artifacts.
- `Renamer.UI` composes core services via DI and presents data.

## Contracts namespace
- Namespace: `Renamer.Core.Contracts`
- Required types:
  - `RenamePlan`
  - `RenamePlanOperation`
  - `RenamePlanReason`
  - `RenamePlanSummary`
  - `RenameReport`
  - `RenameReportResult`
  - `RenameReportSummary`

## Core service interfaces
- `IPlanBuilder`
  - Build in-memory `RenamePlan` from a root path and scan services.
- `IApplyEngine`
  - Execute `RenamePlan` with adaptive retry policy and return in-memory report results.
- `IPlanSerializer`
  - Write/read plan artifacts using schema current contract.
- `IReportSerializer`
  - Write report artifacts using schema current contract.

## Infrastructure interfaces
- `ILogPathProvider`
  - Resolve default long-lived log file path per executable and OS.
- `IClock`
  - Provide UTC timestamps for deterministic tests (`createdAtUtc`, `startedAtUtc`, `finishedAtUtc`).

## Data flow
1. Planner scans root and builds `RenamePlan`.
2. Plan serializer writes `rename-plan.json` (overwrite semantics).
3. Apply engine loads plan, performs renames, enforces retry/abort policy.
4. Report serializer writes `rename-report.json` (overwrite semantics).
5. CLI/UI consume artifacts and optional logs.

## Cross-platform considerations
- Use `Path` APIs and normalize separators.
- Avoid OS-specific invalid characters when generating new names.
- UI target frameworks are desktop-only in v1:
  - macOS via Mac Catalyst
  - Windows
- Default log locations:
  - Windows `%LOCALAPPDATA%/Renamer/logs`
  - macOS `~/Library/Logs/Renamer`
