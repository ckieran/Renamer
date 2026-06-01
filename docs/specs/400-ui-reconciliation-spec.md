# 400 UI Reconciliation Spec

## Slice ID
`400-ui-reconciliation-spec`

## Goal
Ground the next UI cleanup pass in concrete visual references by recording the gap between the current native UI screenshots and the compact concept mockups.

## In scope
- Add this reconciliation spec as the source of truth for the next UI cleanup pass.
- Add a curated PNG reference set under `docs/specs/design-references/400-ui-reconciliation/`.
- Record which design artifacts are canonical:
  - JSX mock source: `docs/specs/mockups/hifi-mocks-v2.jsx`.
  - Renderable mock wrapper: `docs/specs/mockups/Renamer hi-fi.html`.
  - Committed PNG references in this slice for target/current comparison.
- Define follow-up slices `410` to `430` with concrete scope, acceptance themes, and visual verification expectations.
- Add follow-up slice specs `410` to `430` so the deterministic checklist workflow can continue.
- Add `.superpowers/` to `.gitignore` because browser companion sessions are local scratch artifacts.
- Update `docs/specs/000-index.md` and `docs/checklists/v1.md`.

## Out of scope
- Any `src/**` changes.
- Any generated HTML exports from `Renamer_WIP`.
- Any test changes.
- Pixel-perfect token changes; later implementation slices own the exact XAML/layout edits.

## Pre-implementation gate (completed before edits)
1. `git status --short` returned clean after stashing local browser companion artifacts.
2. `git branch --show-current` returned `main`.
3. `git pull --ff-only origin main` completed with `Already up to date.`
4. Created the slice branch:
   - `git switch -c docs/400-ui-reconciliation-spec`
5. Confirmed branch:
   - `git branch --show-current` returned `docs/400-ui-reconciliation-spec`.
6. No matching GitHub issue was found for `400 ui reconciliation spec`.

## Visual References

### Canonical mock source
- `docs/specs/mockups/hifi-mocks-v2.jsx`

This is the canonical editable design artifact because it is compact, reviewable, diffable, and includes Plan, Review, Rename, light/dark, and additional states.

### Renderable mock wrapper
- `docs/specs/mockups/Renamer hi-fi.html`

This wrapper renders the JSX mockups. It may rely on browser-executed React/Babel resources. It is useful for visual review but should not be treated as the source of design truth.

### Committed target references
- `docs/specs/design-references/400-ui-reconciliation/target-plan-defaults-collapsed.png`
- `docs/specs/design-references/400-ui-reconciliation/target-plan-advanced-expanded.png`
- `docs/specs/design-references/400-ui-reconciliation/target-review.png`

### Committed current native references
- `docs/specs/design-references/400-ui-reconciliation/current-plan-dark.png`
- `docs/specs/design-references/400-ui-reconciliation/current-plan-light.png`
- `docs/specs/design-references/400-ui-reconciliation/current-review-dark.png`
- `docs/specs/design-references/400-ui-reconciliation/current-rename-dark.png`

## Reconciliation Findings

The current native UI has implemented many v2 cleanup ingredients, but the rendered composition still differs from the compact concept in ways that matter for usability and visual confidence.

### Shell scale and hierarchy
The concept uses a compact app shell: a small `Renamer` title, a `Plan · Review · Rename` breadcrumb, and a workspace panel that owns the active step title. The current UI still reads like a large landing page, with `Rename your folders in three steps` and explanatory text dominating the first viewport.

Required direction:
- Header title is `Renamer`.
- Step-specific heading lives inside the workspace.
- Page padding and top whitespace should be reduced to match the 1100x720 concept density.
- The workspace should feel like the main app surface, not a framed content island beneath a hero.

### Rail and workspace relationship
The rail is now thin, but the current layout leaves the rail and workspace inside a large bordered shell. In the concept, the rail and workspace are peers inside one dense working area.

Required direction:
- Keep the thin numbered rail.
- Keep stacked-letter labels.
- Tighten rail/workspace alignment so the active workspace starts near the top of the rail content.
- Avoid outer frames that make the whole app feel nested.

### Plan workspace
The concept Plan screen is a task surface: one path row, a compact advanced/defaults area, a status line at bottom-left, and the commit action at bottom-right. The current Plan screen has the right ingredients but too much vertical empty space above the work and too much instructional copy.

Required direction:
- Preserve smart defaults from slice `360`.
- Preserve inline browse from slice `350`.
- Place the primary `Build plan` action in the bottom action area.
- Show the discovered subfolder/status text near the bottom-left action area.
- Keep advanced override controls visually secondary.

### Review workspace
The concept Review screen uses a stat strip and dense bounded operation cards. The current Review screenshot is close, but follow-up work should ensure it matches the target density, card hierarchy, and bottom action placement.

Required direction:
- Keep stat strip and card plan items from slices `370` and `380`.
- Ensure operation cards use from/to hierarchy, status pill placement, and compact spacing from the target.
- Keep back/reload/continue actions in a bottom action row.

### Rename workspace
The current Rename screen should converge on the same shell and action-row decisions. The apply/result sections should use the compact result banner direction from slice `380`, without reintroducing large explanatory blocks.

Required direction:
- Keep apply actions clearly gated.
- Keep result summary compact.
- Use the same shell, workspace, and bottom action conventions as Plan and Review.

## Visual Feedback Workflow

Use two feedback loops:

1. Browser companion for fast design review.
   - Use it for side-by-side mockups, layout options, and spec discussion.
   - Treat it as scratch. Do not commit `.superpowers/` session files.

2. Native app screenshots for implementation acceptance.
   - For each UI implementation slice, run the MAUI app and capture light/dark screenshots for the affected workspace.
   - Computer Use is appropriate when Codex needs to operate or inspect the running native app.
   - Browser mockups are not final proof; native screenshots are.

Each follow-up UI slice must include a PR note listing:
- target reference image(s) used,
- current screenshot(s) captured after implementation,
- any intentional deviation from the concept.

## Follow-up Slice Map

| Slice | Type | Scope |
|---|---|---|
| `410` | `refactor(ui)` | Compact shell and workspace frame. Replace landing-scale header with compact app header, reduce page padding, remove unnecessary outer framing, and align rail/workspace density to the concept. |
| `420` | `refactor(ui)` | Plan workspace reconciliation. Align the Plan screen to the target: one compact path row, secondary advanced defaults, bottom status/action row, and reduced instructional copy. |
| `430` | `refactor(ui)` | Review/Rename workspace reconciliation. Align stat/card density, action rows, result banner, and shell consistency across Review and Rename. |

## Commands to run
1. `dotnet restore Renamer.sln`
2. `dotnet build Renamer.sln`
3. `dotnet test Renamer.sln`

## Git/PR workflow
1. Branch from current `main` using the conventional commit type prefix `docs/`.
2. Keep PR scoped to docs, committed design reference PNGs, `.gitignore`, checklist, and index updates.
3. PR title: `docs(specs): 400 ui reconciliation spec`.
4. If a matching issue is created later, link it from the PR body.

## Acceptance checks
- This spec exists and records canonical mockup sources, committed target references, and committed current references.
- `docs/specs/design-references/400-ui-reconciliation/` contains the curated PNG reference set.
- `docs/checklists/v1.md` includes `400` checked and follow-up slices `410` to `430` unchecked.
- `docs/specs/410-compact-shell-workspace-frame.md`, `420-plan-workspace-reconciliation.md`, and `430-review-rename-workspace-reconciliation.md` exist.
- `docs/specs/000-index.md` references `400`.
- `.gitignore` ignores `.superpowers/`.
- No `src/**` files are modified.

## Tests
- None. This is a documentation-only slice.

## Test scope
- Standard restore/build/test still pass after docs-only changes.

## Expected outputs
- `docs/specs/400-ui-reconciliation-spec.md`
- `docs/specs/design-references/400-ui-reconciliation/*.png`
- `docs/specs/410-compact-shell-workspace-frame.md`
- `docs/specs/420-plan-workspace-reconciliation.md`
- `docs/specs/430-review-rename-workspace-reconciliation.md`
- `docs/checklists/v1.md`
- `docs/specs/000-index.md`
- `.gitignore`

## Definition of Done
- Acceptance checks are satisfied.
- Standard command sequence passes locally.
- Checklist entry for `400` is checked before PR.
- Branch pre-implementation gate completed before first edit.
- PR scope is limited to this docs-only slice.
