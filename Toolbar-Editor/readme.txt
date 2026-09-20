TOOLBAR EDITOR
==============

Toolbar Editor turns the Captain of Industry toolbar into a complete visual
editor without replacing the menus, buttons, text, or actions that already
exist.

GETTING STARTED
---------------

- Third-party top-level menus keep the same default section assignment their
  mods registered, now displayed through Toolbar Editor's six fixed mirror hosts.
- Named profiles are stored independently in Toolbar Editor's own profile folder;
  legacy profile folders are not imported automatically.
- Left-click the Toolbar Editor icon in its own normal toolbar section to
  open or close Toolbar Editor.
- Right-click the Toolbar Editor icon to toggle Edit Mode immediately.
- The optional T button beside the calendar provides the same access if the
  Toolbar Editor icon is moved out of reach.
- Ctrl + [ opens or closes Editor & Settings, matching the icon's left-click.
- Ctrl + ] toggles Edit Mode, matching the icon's right-click.
- Keybind Framework adds a dedicated Toolbar Editor tab for rebinding both
  shortcuts. Without it, the Ctrl-bracket defaults remain fully operational.
- When Buttons is installed, a separate calendar shortcut appears after the
  existing Tweaks++ open and HUD-lock controls. It opens Buttons without
  changing either Tweaks++ action.

LAYOUT EDITOR
-------------

Select [Edit layout] to highlight editable toolbar components. The default
controls are:

- Ctrl + Left-drag: Move a section anywhere within the screen boundaries.
- Shift + Left-drag: Resize the width and height of a supported non-section
  panel together. The Submenu and item panel remains width-only.
- Ctrl + Shift + Left-drag: Drag and drop buttons to reorder them or move them
  between sections.
- Ctrl + Shift + Alt + Left/Right-click: Bring a section or the search panel
  forward, or send it backward.

Open Layout > Controls and enable Reorder to move these action cards between
the eight fixed No modifier, Ctrl, Shift, Alt, and combined-modifier rows.
No modifier means plain left-click and remains unassigned by default. Moving
onto an occupied row swaps the actions. Unused rows hide when Reorder closes
and reappear the next time it is enabled.
This fixed-row design prevents two actions from receiving the same control.
No modifier is an advanced option: while Edit Mode is active it can replace a
toolbar component's normal primary-click action, and its drag actions require
Mute Fine-Tune selection clicks to be off. Modifier-bound controls do not have
those limitations.

Additional Layout Editor features:

- Toggle Edit Mode from the title bar while using any settings tab.
- Move sections together with the toolbar background, or leave them fixed.
- Auto-Shift is the protected default-style one-row mode. It keeps the Toolbar
  background, sections, and Search managed together and keeps editable sections
  horizontal. Section movement remains available for alignment corrections.
- While Auto-Shift is on, the Resize panel action is blocked for the toolbar
  background and Search so their managed flow cannot be resized into an invalid
  layout. Its control card appears muted but remains reorderable. Submenu
  resizing and all Move component dragging remain available.
- Turning Auto-Shift off is a confirmed transition that freezes the current
  Toolbar background, Search, and section geometry for vertical rails, multiple
  rows, or other free-form layouts. New and restored sections then appear near
  the lower center.
- Rebuild Auto-Shift layout remains available whether Auto-Shift is on or off.
  It restores the managed row, Search edge, and horizontal section orientation
  while preserving existing sections, names, borders, button assignments,
  button order, colors, and hidden state.
- Scale the Toolbar background and all toolbar sections together from 50% to
  200%.
- Scale the Ctrl+F search control independently from 50% to 200%.
- Scale the submenu and item panel independently from 76% to 200%.
- Add up to twelve user-custom sections. The normal editor budget also includes
  the six fixed middle defaults, two native green tool-shortcut hosts, and any
  additional native or modded sections the game registers. Those unavoidable
  native sections remain supported even if they push the live count above the
  nominal twenty-section editor budget; the custom-section limit remains twelve.
- Rename or switch any of the six fixed Toolbar Editor default sections and any
  user-custom section between a horizontal row and a natural one-column
  vertical rail from the Sections tab.
- Keep the game's six original middle section hosts alive but empty and hidden;
  Toolbar Editor's fixed mirrors reuse the same live buttons and controllers.
- Keep both green native tool-shortcut hosts live while allowing them to be
  moved, layered, identified, and scaled independently through Fine Tune.
- Remove the last empty section without deleting occupied sections.
- Keep empty sections available in the editor and recovery inventory while
  collapsing their unused space on the live HUD outside Edit Mode.
- Let native, custom, and toolbar-shortcut sections grow or shrink naturally
  with their button contents.
- Present the native green tool-shortcut section as a true one-column rail. Its
  current eight shortcuts form an 8 x 1 layout without shrinking their icons,
  glow artwork, or click targets, and additional shortcuts extend it vertically.
- Keep normal native and custom sections at their natural content size even
  when the decorative toolbar background is made narrower than its contents.
- Automatically return a suppressed native section when newly unlocked content
  needs its original toolbar home.
- Show or hide individual toolbar buttons from a collapsible list.
- Reset individual components or the standard toolbar sections with the
  available reset controls. Reset All restores every component, including all
  six yellow sections, without requiring a second section reset. Reset all
  sections preserves custom section names and button assignments; the full
  layout reset restores default names.
- Open the Fine Tune Toolbar Components tray from the gold arrow above the
  footer. Its open state is remembered globally between games.
- Select any active, usable registered component from the Fine Tune component
  dropdown, including one that is currently off-screen or difficult to click
  directly. Suppressed recovery-only sections remain available in Sections.
- While Edit Mode and the Fine Tune tray are open, plain left-click a live
  toolbar component to select it. Mute clicks prevents that same click from
  activating the component's normal action and can be changed from the tray or
  Layout panel. Modifier-bound Edit Mode controls remain available either way;
  No modifier drag actions require Mute clicks to be off.
- A green Selection Mode indicator remains visible while Fine Tune selection is
  active. Closing the tray or editor disables selection without ending Toolbar
  Edit Mode.
- Fine-tune the selected component with reset, identify, one-pixel nudging,
  supported layer controls, exact X/Y screen-position input, and a one-click
  Horizontal/Vertical switch for Toolbar Editor custom sections.
- Move and scale the native tool filters and auxiliary controls independently.
- Select the separate Toolbox shortcuts target to move and scale active-tool
  shortcut bars, including full transport controls and compact one-button
  variants.
- Move and scale the Belts & pipes snapping indicator, Planning Mode indicator,
  Toolbar Edit Mode banner, Toolbar Selection Mode banner, and Overlays panel.
  Their scale is included in saved profiles.
- Component dragging is substantially smoother; Fine Tune position fields and
  profile state refresh after the drag is released.
- Reset Component uses the game's compact inline confirmation prompt beside the
  invoking control instead of opening a separate movable window.
- Resize the editor vertically from the full-width bottom resize strip. The
  saved height is the closed-tray size, so opening the tray does not overwrite
  the user's preferred editor height.
- Use the title-bar minimize button for the useful 480-pixel selection-list
  height, or manually resize as low as a 100-pixel base height when only the
  title, footer, and Fine Tune controls are needed. Restore returns to 740.
- Choosing Review & save from the unsaved-change prompt opens Profiles at its
  save row and temporarily fits a too-short editor without replacing the
  recalled custom height.
- Screen-edge limits and cancelled invalid drops keep components and buttons
  recoverable.

CUSTOMIZE SECTIONS
------------------

- Select any native, custom, or green tool-shortcut section from the compact
  dropdown without scrolling through a separate section list.
- The selected-section statistics panel shows its type, X/Y position,
  effective scale, orientation, layer order, and assigned buttons. Hidden
  assignments are labeled so an apparently empty undeletable section can be
  diagnosed directly.
- Click the green Identify button to briefly display the selected section's
  name directly on its live toolbar panel.
- Rename native or custom sections to keep complex layouts understandable.
- Custom section names persist with the live layout and are included in
  .toolbar profile files.
- Custom-section orientation persists with the current game and in .toolbar
  profiles. Profiles created before v1.5.0a safely load as horizontal.
- Reset Section restores that section's position, supported scale, layer, and border;
  its custom name, orientation, and assigned buttons are preserved.
- Cycle the selected section's bottom border through Off, Concave, and Convex.

SETTINGS AND TOOLBAR PROFILES
-----------------------------

- Choose the globally saved Toolbar Editor shortcut icon from the compact
  Settings dropdown. The live shortcut
  changes immediately without losing its current section, order, or position.
- Custom artwork is framed to its visible alpha bounds and rendered at the
  game's native medium icon size inside a fixed normal toolbar-button footprint.
- The Settings icon selector includes a dedicated shortcut color picker. The
  artwork blends with the selected tint.
- The included choices are Burning T and Industrial Toolbox.
- The active toolbar is stored in the current Captain of Industry save.
  Different saves can keep completely different layouts without inheriting
  the last world that was played.
- Normal Save, Save As, and autosave capture the current toolbar. Toolbar
  edits made after the last game save are intentionally discarded if that
  game is abandoned without saving.
- New, legacy, and reset-to-default saves remain on the native default layout
  until a toolbar customization or named profile is deliberately applied.
- Save the current arrangement as a reusable .toolbar profile.
- Named .toolbar profiles remain global templates: loading one changes only
  the current save's active layout, and the game must then be saved to keep it.
- Select, load, overwrite, rename, or permanently delete saved profiles.
- Profile rows separately mark Selected, Active, or Active + Selected state.
- New and overwritten profiles show the game UI scale used when they were
  saved. Legacy profiles remain loadable with an unknown scale tag.
- The Status panel reports successful actions and errors.
- Copy the profiles folder path for File Explorer or profile sharing.
- Use Refresh after adding, removing, or renaming .toolbar files outside the
  game.
- Choose whether both editor buttons, only the calendar button, or only the
  toolbar icon remain visible without affecting either bracket shortcut.

LOCALIZATION
------------

Toolbar Editor supports all 21 Captain of Industry languages: English,
Catalan, Czech, Dutch, Estonian, French, German, Hungarian, Italian, Japanese,
Korean, Norwegian Bokmål, Polish, Brazilian Portuguese, Russian, Spanish,
Swedish, Turkish, Ukrainian, Simplified Chinese, and Traditional Chinese.

Tabs, page content, prompts, status messages, and tooltips can use the active
catalog. The Fine-Tune Controller deliberately keeps its visible labels,
dropdown text, and button text fixed in English so translated word length
cannot disturb its compact layout; its tooltips and confirmation prompts can
still be translated.

COMPATIBILITY
-------------

Toolbar Editor reuses each original live toolbar button. Menu names, category
IDs, icons, tooltips, actions, research unlocks, selected states, visibility
rules, and translations remain owned by Captain of Industry and the mod that
registered them.

Toolbar Editor stores its active layout as removal-safe data inside each game
save. Keybind Framework is optional, and Toolbar Editor can still be added to
or removed from an existing save.
