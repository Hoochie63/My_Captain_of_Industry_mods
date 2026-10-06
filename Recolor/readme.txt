RECOLOR v1.0.8 — GROUPED TEMPLATE LIBRARY AND AREA PAINTING

Recolor gives placed buildings independent colors without changing their shared vanilla source textures.

GETTING STARTED

- Open Recolor from its green paintbrush button beside the calendar controls.
- Right-click that button to open the expanded Mesh Templates library directly.
- Select a supported placed building, choose one of 16 curated presets, enter an exact six-digit Hex color, or adjust red, green, blue, saturation, brightness, and tint.
- Apply changes directly to the selected building or use the footer tools to work across the island.
- Original restores the active whole-building draft or selected mesh group to its original appearance. Its active indication makes an Original draft clear before it is applied.
- Save up to 40 exact colors with the centered green star. Left-click a Favorite to load it and right-click it to remove it. Favorites are portable across worlds.

CUSTOM WORLD TOOLS

- Camera controls remain available during paint-tool drags and batch processing, using the game's configured camera bindings and speed modifier.

- Select Without Inspector defaults on and is remembered. While Recolor is open, it selects supported buildings without opening their normal inspector panels. Closing Recolor suspends this behavior; reopening it restores the remembered choice.
- Global Paint left-clicks a supported building with the active palette color. Right-click picks that building's Global Color into the palette. Applying a Global Color replaces that target's Mesh Overrides.
- Mesh Eyedropper starts in source-pick mode. Left-click a source building to capture its Global Color and complete Mesh Override set; the cursor then becomes a brush. Left-click the same building prototype to apply the complete result.
- Global Paint and the armed Mesh Eyedropper also support area dragging. Source sampling remains single-building only. The selection shows compatible building counts and accepts at most 256 buildings per drag.
- One batch runs at a time in small bursts. Right-click cancels its unfinished remainder; Escape or switching tools cancels the tool. Already painted buildings retain their colors. Heavy buildings can still take time; start with smaller selections.
- Both paint tools work with the menu closed, including activation from the optional toolbar shortcuts. Closing Recolor keeps the tool, its sampled colors, and any pending batch active. Use Escape or switch tools to stop painting. The palette retains the active Global Paint color when reopened.
- Exact mesh schemas remain the fast path. Saved templates can recover complete, uniform overridden groups after harmless sibling-order or untouched-group drift, but missing, partial, conflicting, or ambiguous mappings are refused instead of being partially recolored.
- Right-click while the Mesh Eyedropper is armed returns to source-pick mode. Right-click again while it is waiting for a source exits the tool. Escape exits either paint tool immediately without clearing the remembered Select Without Inspector choice.
- Global Paint and Mesh Eyedropper use Captain of Industry's native cursors, supported-building highlights, and structured hover panels. Their primary and secondary action legends come from the game's remap-aware KeyBindUi labels.
- The Settings tab can opt into Global Color transfer through the game's native Apply Settings eyedropper. It is off by default and does not copy Mesh Overrides.
- The Settings tab also has independent, off-by-default options for native bottom-left Global Paint and Mesh Eyedropper shortcuts. They reuse the existing tools and can be discovered by compatible keybind mods.

EDIT MESH

- Press More in the footer to open Edit Mesh, Mesh Templates, and Settings. The footer keeps only Select Without Inspector, Global Paint, Mesh Eyedropper, flexible spacing, and More.
- Edit Mesh includes an isolated 3D draft preview. The selected mesh shows the current uncommitted palette draft before Apply while its other groups retain their current Global Color and Mesh Overrides.
- Drag the preview to rotate it, scroll to zoom, use the paired side controls for precise 45-degree rotation, or reset to a centered front view. Reset View also rebuilds the selected building's live mesh list. The preview owns its temporary renderer materials and textures so restoring a live building cannot invalidate it.
- Matching LOD0, LOD1, LOD2, and other distance variants are combined into logical mesh groups while distinct body, pavement, liquid, glass, icon, signage, and detail regions remain independent.
- The selected group area keeps its effective-color state and building identity together on one row, with logical group totals on the second row. Low-level renderer, material, texture, and path diagnostics stay out of the routine editing surface.
- Selecting a group preserves the active palette draft so the same color can be applied rapidly across several meshes. Use the compact previous/next buttons at the start of the selector row for sequential work, or click the selected group's color swatch when its effective color should be loaded into the palette. Apply updates every tintable linked slot in that group. Original restores its native appearance. Use Global Color removes only that group's override, and Use Global Color for All removes every override after a native confirmation.
- Undo restores the most recent mesh edit on the selected building. The corrected native Undo and paired preview-control icons remain readable at supported Recolor scales.
- Whole-building Apply, Original, and Paste ask for confirmation only when they would remove existing Mesh Overrides.
- Save Template saves the selected building's applied colors directly from Edit Mesh; unapplied palette edits are not included.

MESH TEMPLATE LIBRARY

- Saved templates are grouped by exact building prototype, not display name. The main card represents the first template; expand a group to see its other color variants without duplicating that first card.
- Drag the native handles to reorder building groups or variants. No Reorder toggle is needed. Other variants can be dragged onto the main card to make them first. Groups move together with their variants.
- Expand Library switches the same window to a larger library with up to four columns. Back to Recolor or Escape restores the normal view. A native maximize icon also toggles this view while More or the library is open; it is hidden in palette-only view.
- Save and navigation controls stay above the card scroller. Only one group expands at a time, and variants fill the available columns before later groups.
- Load prepares Mesh Eyedropper at its application stage without leaving the current view by default. Settings > Mesh Templates offers Return to Edit Mesh after loading a template; enabling it returns to Edit Mesh after a successful load. The choice is remembered between sessions. Templates retain their saved display names; hovering a clipped name reveals it. The compact numbers show overridden mesh groups / total mesh groups.
- Expanding variants keeps that group fully visible while other cards are 50% opaque and still usable. Collapse the group to restore full opacity. Newly visible variant previews refresh after layout without needing a scroll.

CURRENT SCOPE

- Global Colors and per-building Mesh Overrides are stored in the world save and restored when the world is loaded. Recolor can be safely added to and removed from an existing saved game.
- A live-captured Mesh Eyedropper payload remains only in the active tool session; applying it writes the resulting Mesh Overrides to the target building's saved state.
- Mesh Templates store complete Global Color and Mesh Override payloads with the world. Libraries support up to 512 templates within the existing save-size envelope. Thumbnails render only around visible rows in paced steps; the cache is bounded independently of library size. Transferring the same world save carries its templates, rather than sharing one library across different worlds.
- Recolor normally reopens on Edit Mesh instead of immediately rebuilding the heavier Mesh Templates view. Right-clicking its calendar button explicitly opens the expanded library.
- Transport networks, roads, bridges, and train tracks remain intentionally excluded.
- Interface text and hover explanations are localized for all 21 languages shipped with Captain of Industry 0.8.7b.
- Recolor's title-bar percentage control scales only Recolor and its prompts from 65% to 120%; ordinary game tooltips retain Captain of Industry's global interface scale.
- Compatibility verified against Captain of Industry 0.8.7d. This release does not include the abandoned instanced-building recoloring experiment.
