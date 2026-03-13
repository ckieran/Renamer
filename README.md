# Renamer

Renamer is a .NET desktop and CLI tool for reorganizing local photo folders by EXIF capture-date range. It scans supported image files, builds a deterministic rename plan, lets you review that plan before any filesystem changes are made, then applies the rename operations and records a machine-readable report.

## Current Status

- Core planning and apply logic is implemented in `Renamer.Core`.
- CLI plan/apply commands are implemented.
- Desktop UI can load and preview an existing `rename-plan.json` artifact.
- Desktop UI can apply a loaded plan and display report results.
- Desktop UI plan generation is planned next and tracked as slice `250`.

## Supported Scope

### Platforms

- Windows
- macOS

### File types

- Nikon RAW: `.nef`
- JPEG: `.jpg`, `.jpeg`

### Artifacts

- Plan artifact: `rename-plan.json`
- Execution report: `rename-report.json`

## Workflow

1. Generate a rename plan from a root folder.
2. Review the planned operations before making changes.
3. Apply the plan to rename folders.
4. Inspect the execution report and logs.

Folder names follow the current v1 contract:

- Range: `YYYY-MM-DD - YYYY-MM-DD - <original folder name>`
- Single day: `YYYY-MM-DD - <original folder name>`

On destination conflicts, execution retries deterministically with suffixes such as ` (1)`, ` (2)`, up to 10 suffix retries.

## Repository Layout

- `src/Renamer.Core` - core planning, apply, serialization, logging, and contracts
- `src/Renamer.Cli` - command-line wrapper over core services
- `src/Renamer.UI` - .NET MAUI desktop UI wrapper
- `src/Renamer.Tests` - unit and UI ViewModel tests
- `docs/specs` - canonical specs and slice definitions
- `docs/checklists` - delivery tracking

## Development

### Prerequisites

- .NET 10 SDK

### Additional setup by work area

#### Core, CLI, and test work

No extra platform tooling is required beyond the .NET 10 SDK. Contributors working only in `Renamer.Core`, `Renamer.Cli`, or `Renamer.Tests` should be able to use the standard `dotnet restore`, `dotnet build`, and `dotnet test` workflow without installing MAUI desktop dependencies.

#### Desktop UI work

`Renamer.UI` is a .NET MAUI desktop app and requires MAUI platform tooling in addition to the .NET 10 SDK.

- macOS: Xcode and Mac Catalyst support
- Windows: Windows desktop development tooling and a compatible Windows SDK
- .NET MAUI workload for .NET 10

The UI project currently targets:

- `net10.0-maccatalyst` on macOS
- `net10.0-windows10.0.19041.0` on Windows

If you do not need to build or run `Renamer.UI`, you can usually avoid the extra MAUI-specific setup.

#### Optional workflow tooling

- GitHub CLI (`gh`) for the repo's issue and pull request workflow

### Common commands

```bash
dotnet restore Renamer.sln
dotnet build Renamer.sln
dotnet test Renamer.sln
```

### Project workflow

Development is spec-driven and executed in small slices:

- canonical specs live under `docs/specs/`
- delivery tracking lives under `docs/checklists/`
- one slice is implemented per branch and per PR
- core behavior should stay in `Renamer.Core`, with CLI and UI acting as wrappers

This repository also includes AI-assisted development guidance in `AGENTS.md`. The source of truth remains the checked-in specs, code, tests, and review process.

## Key Docs

- [Delivery checklist](docs/checklists/v1.md)
- [Product spec](docs/specs/010-product.md)
- [Architecture](docs/specs/040-architecture.md)
- [Workplan](docs/specs/050-workplan.md)
- [Engineering contract](docs/specs/070-engineering-contract.md)

## Current Limitations

- The desktop UI does not yet generate plans directly from a selected root folder; it currently starts from an existing plan artifact.
- Cross-platform smoke validation is still pending in the delivery checklist.
- v1 is limited to local filesystem workflows and the supported file types listed above.

## License

This project is licensed under the MIT License.
