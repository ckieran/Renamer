# 130 EXIF Reader JPEG/NEF

## Goal
Read capture dates from `.jpg`, `.jpeg`, `.nef` and classify missing EXIF safely.

## In scope
- Supported extension checks for v1 formats.
- EXIF extraction returning optional capture date.
- Warning classification for missing/invalid EXIF.

## Out of scope
- Folder range aggregation.
- Rename planning.

## Implementation steps
1. Add/confirm EXIF service abstraction in core.
2. Implement extension allow-list (`.jpg`, `.jpeg`, `.nef`).
3. Parse EXIF capture date and return null on missing/invalid data.
4. Return warning metadata (or warning count) for skipped files.
5. Ensure warnings are consumable by planner summary (`filesSkippedMissingExif`).

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~Exif"`

## Acceptance checks
- Valid JPEG/NEF metadata returns capture date.
- Missing/invalid EXIF returns no date and warning path.
- Warning counts can be surfaced into plan `reason.filesSkippedMissingExif`.

## Tests
- Add/update `src/Renamer.Tests/Core/ExifServiceTests.cs`.

## Test scope
- EXIF parsing and extension validation only.

## Expected outputs
- EXIF service implementation in core.
- EXIF warning-aware tests.

## Exit criteria
- EXIF service behavior is deterministic and tested.
