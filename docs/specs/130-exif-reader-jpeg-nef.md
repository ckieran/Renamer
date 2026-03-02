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

## Acceptance checks
- Valid JPEG/NEF metadata returns capture date.
- Missing/invalid EXIF returns no date and warning path.

## Tests
- Unit tests for supported extensions and EXIF extraction outcomes.

## Exit criteria
- EXIF service behavior is deterministic and tested.
