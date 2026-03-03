# Product Spec (Draft)

## Goal
Provide a cross-platform desktop experience for renaming photo folders based on EXIF capture-date ranges, with a safe preview step before any changes are made.

## Non-goals (v1)
- No cloud storage or remote libraries.
- No photo previews or thumbnails.
- No custom naming templates beyond the default date-range format.
- No mobile/tablet app targets (iOS, iPadOS, Android).

## Primary user
Photo owners who manage local folders (RAW + JPEG) and want consistent date-based folder names.

## Core value
Reduce manual folder renaming by providing an accurate, previewable, and auditable rename plan.

## Supported platforms (v1)
- Windows
- macOS

## Supported file types (v1)
- RAW: `.nef` (Nikon)
- JPEG: `.jpg`, `.jpeg`

## Primary flow
1. Select a root folder.
2. Scan folders and extract capture dates.
3. Compute per-folder date range.
4. Show preview (planned folder rename operations).
5. Apply rename plan.
6. Record results to log.
