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
- Left-click the Toolbar Editor icon in the primary tool-shortcut section to
  open or close Toolbar Editor.
- Right-click the Toolbar Editor icon to toggle Edit Mode immediately.
- The optional T button beside the calendar provides the same access if the
  Toolbar Editor icon is moved out of reach.
- Ctrl + [ opens or closes Editor & Settings, matching the icon's left-click.
- Ctrl + ] toggles Edit Mode, matching the icon's right-click.
- Keybind Framework adds a dedicated Toolbar Editor tab for rebinding both
  shortcuts. Without it, the Ctrl-bracket defaults remain fully operational.

LAYOUT EDITOR
-------------

Select [Edit layout] to highlight editable toolbar components, then use:

- Ctrl + Left-drag: Move a section anywhere within the screen boundaries.
- Shift + Left-drag: Resize the width of a supported non-section panel (Search,
  toolbar background, or the Submenu and item panel).
- Alt + Left-drag: Resize the height of a supported non-section panel (Search,
  or the toolbar background). The Submenu and item panel supports width
  resizing only.
- Ctrl + Shift + Left-drag: Drag and drop buttons to reorder them or move them
  between sections.
- Ctrl + Shift + Alt + Left/Right-click: Bring a section or the search panel
  forward, or send it backward.

Additional Layout Editor features:

- Toggle Edit Mode from the title bar while using any settings tab.
- Move sections together with the toolbar background, or leave them fixed.
- Auto-Shift is intended for initial default-style one-row layouts while using
  button drag-and-drop. Turn it off before manually moving a section, creating
  vertical rails, or building multiple rows. Existing components then stay
  fixed and genuinely new sections appear near the lower center.
- Scale the complete toolbar from 50% to 200%.
- Scale the Ctrl+F search control independently from 50% to 200%.
- Scale the submenu and item panel independently from 76% to 200%.
- Add up to twelve user-custom sections. Together with the six fixed middle
  defaults and two native green tool-shortcut hosts, Toolbar Editor supports
  up to twenty active section slots.
- Rename or switch any of the six fixed Toolbar Editor default sections and any
  user-custom section between a horizontal row and a natural one-column
  vertical rail from the Sections tab.
- Keep the game's six original middle section hosts alive but empty and hidden;
  Toolbar Editor's fixed mirrors reuse the same live buttons and controllers.
- Keep both green native tool-shortcut hosts live while allowing them to be
  moved, layered, identified, and scaled independently through Fine Tune.
- Remove the last empty section without deleting occupied sections.
- Keep empty custom sections visible until you explicitly remove them.
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
  available reset controls. If the full-toolbar reset leaves section transforms
  behind after a loaded profile, use Reset all sections afterward.
- Open the Fine Tune Toolbar Components tray from the gold arrow above the
  footer. Its open state is remembered globally between games.
- Enable Selection Mode in that tray, then plain left-click a live toolbar
  component to select it without activating its normal action. Existing
  Ctrl/Shift/Alt editing controls remain available while Selection Mode is on.
- A green Selection Mode indicator remains visible while the mode is
  active; close it with its X or toggle Selection Mode off from the tray.
- Fine-tune the selected component with reset, identify, one-pixel nudging,
  supported layer controls, exact X/Y screen-position input, and a one-click
  Horizontal/Vertical switch for Toolbar Editor custom sections.
- Reset Component uses the game's compact inline confirmation prompt beside the
  invoking control instead of opening a separate movable window.
- Resize the editor vertically from the full-width bottom resize strip. The
  saved height is the closed-tray size, so opening the tray does not overwrite
  the user's preferred editor height.
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
- The Settings icon selector includes a dedicated shortcut color picker. White
  icons recolor directly, while colored artwork blends with the selected tint.
- The included choices are Compact Block Hammer, Original Detailed Hammer,
  Soft Mallet, Outline Mallet, Inferno T, Inferno Double Hammer T, Burning T,
  and Industrial Toolbox.
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
- The Status panel reports successful actions and errors.
- Copy the profiles folder path for File Explorer or profile sharing.
- Use Refresh after adding, removing, or renaming .toolbar files outside the
  game.
- Choose whether both editor buttons, only the calendar button, or only the
  toolbar icon remain visible without affecting either bracket shortcut.

LOCALIZATION
------------

Toolbar Editor currently presents its editor in English.

COMPATIBILITY
-------------

Toolbar Editor reuses each original live toolbar button. Menu names, category
IDs, icons, tooltips, actions, research unlocks, selected states, visibility
rules, and translations remain owned by Captain of Industry and the mod that
registered them.

Toolbar Editor stores its active layout as removal-safe data inside each game
save. Keybind Framework is optional, and Toolbar Editor can still be added to
or removed from an existing save.
