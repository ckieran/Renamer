# 150 Name Generation

## Goal
Generate folder destination names using date-first format and unchanged source folder name.

## In scope
- Multi-day format: `YYYY-MM-DD - YYYY-MM-DD - <rest of name>`.
- Single-day format: `YYYY-MM-DD - <rest of name>`.
- `<rest of name>` equals current folder name unchanged.

## Out of scope
- Conflict suffixing.
- Apply execution.

## Implementation steps
1. Add naming service/helper in core.
2. Accept source folder name plus min/max dates.
3. Emit single-day or range format based on min==max.
4. Preserve source folder text verbatim as `<rest of name>`.
5. Add tests with spaces/punctuation in folder names.

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~NameGeneration"`

## Acceptance checks
- Single-day and multi-day examples format correctly.
- Original folder text is preserved verbatim.

## Tests
- Add `src/Renamer.Tests/Core/NameGenerationTests.cs`.

## Test scope
- Naming helper only.

## Expected outputs
- Naming helper implementation and tests.

## Exit criteria
- Naming function is deterministic and tested.
