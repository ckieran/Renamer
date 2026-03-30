# Engineering Contract (Draft)

## Purpose
Define shared execution standards for every implementation slice so PRs are consistent and low-friction.

## Baseline stack
- Runtime and SDK: .NET 9
- Unit tests: xUnit
- Logging: Serilog
- EXIF parsing: MetadataExtractor

## Required behavior
- Core logic remains in `Renamer.Core`.
- CLI and UI are wrappers over core behavior.
- Logging is enabled from startup in each executable:
  - Console sink
  - One long-lived log file per executable

## Log path resolution contract
- Resolve through `ILogPathProvider`.
- Defaults:
  - Windows: `%LOCALAPPDATA%/Renamer/logs/<executable>.log`
  - macOS: `~/Library/Logs/Renamer/<executable>.log`

## Standard command sequence
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

## Exit code contract
- `0`: success
- `2`: invalid arguments or validation failure
- `3`: IO/path access failure
- `4`: unsupported or invalid plan schema
- `5`: conflict retry limit reached (plan execution abort)
- `6`: unexpected runtime error

## Slice execution rules
- Execute one slice per PR.
- Use the slice document as the sole scope for implementation.
- Run the commands listed in the slice plus the standard command sequence.
- when adding dependencies from external sources eg nuget, ask for approval and list popularity/install base.  Preference is for open source recently updated packages with high usage to lower chance of adding malware/vulnerabilities

## Test failure policy

- If any step in the standard command sequence fails, stop and fix before opening a PR.
- A PR must not be opened with a failing build or failing tests.
- All three steps (`restore`, `build`, `test`) are blocking — none may be skipped or bypassed.

## Slice completion requirement
- Every slice must include:
  - implementation steps
  - commands to run
  - test scope
  - expected outputs

## PR Definition of Done
- Slice acceptance checks are satisfied.
- Required tests pass locally.
- `docs/checklists/v1.md` is updated for the slice.
- No unrelated refactors or drive-by changes.

## Test strategy guidance
- Prefer unit tests in early slices.
- Add CLI integration tests for `plan`/`apply` slices.
- For UI, prefer ViewModel/service-level tests plus targeted smoke checks.

## Conventional Commits

All commits in this repository must follow the [Conventional Commits v1.0.0](https://www.conventionalcommits.org/en/v1.0.0/) specification.

### Commit message format

```
<type>(<scope>): <short description>

[optional body]

[optional footer(s)]
```

- The **type** and **short description** are mandatory.
- The **scope** is optional but recommended; use the project layer (`core`, `cli`, `ui`, `tests`) or a short noun identifying the affected area.
- The **short description** is lowercase, imperative mood, no trailing period.
- The **body** is free prose explaining *why*, not *what*. Wrap at 72 characters.
- Breaking changes must include `BREAKING CHANGE: <description>` in the footer, or `!` after the type: `feat!: ...`.

### Type vocabulary

| Type | When to use |
|---|---|
| `feat` | A new feature or capability visible to the user |
| `fix` | A bug fix |
| `refactor` | Code restructuring with no behaviour change (e.g. extracting strings to resources) |
| `docs` | Documentation-only changes |
| `test` | Adding or correcting tests with no production code change |
| `build` | Build system, project file, or dependency changes |
| `ci` | CI/CD pipeline configuration |
| `chore` | Housekeeping that fits no other type (e.g. updating `.gitignore`) |
| `perf` | Performance improvement with no behaviour change |
| `style` | Formatting, whitespace — no logic change |

### Branch naming

Branches must use the conventional commit type as a prefix, followed by the slice ID and a short description:

```
<type>/<slice-id>-<short-description>
```

Examples:
- `feat/130-exif-reader`
- `refactor/290-string-resources`
- `fix/160-conflict-retry-off-by-one`
- `docs/040-architecture-update`

The branch type must match the primary commit type used in that branch. Agent-created branches (e.g. prefixed `claude/`) are permitted for exploration and spec work but implementation branches must follow this convention before the first code commit.
