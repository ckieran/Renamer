# 140 Folder Date Range

## Goal
Compute folder-level min/max capture date from photo metadata.

## In scope
- Range calculation from valid capture dates.
- Ignoring photos without capture date.
- Non-plannable marker for folders with zero valid capture dates.

## Out of scope
- Destination naming.
- Conflict handling.

## Implementation steps
1. Add a range-calculation service/helper in core.
2. Accept photo metadata list and filter to valid dates.
3. Return `(min, max)` for non-empty valid set.
4. Return non-plannable result for empty valid set.
5. Unit-test boundary cases (single date, many dates, none valid).

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln --filter "FullyQualifiedName~DateRange"`

## Acceptance checks
- Mixed valid/missing dates produce correct min/max.
- All-missing folders are flagged as non-plannable.

## Tests
- Add `src/Renamer.Tests/Core/DateRangeTests.cs`.

## Test scope
- Date range logic only.

## Expected outputs
- Date range service/helper implementation.
- Date range unit tests.

## Exit criteria
- Date range logic passes tests.
