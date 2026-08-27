KEYBOUND
Author: Underlörd

KeyBound gives Captain of Industry buttons proper shortcuts and brings Controls
and Framework keybinds together in one profile manager. It discovers supported
toolbar menus, submenus, tools, items, buildings, UI panels, calendar buttons,
and compatible mod-added buttons.

REQUIREMENTS
------------

- Captain of Industry 0.8.5 through 0.8.7b.
- Keybind Framework 2.0.2 or newer.

INSTALLATION
------------

Extract the KeyBound folder into:
C:\Users\[User]\AppData\Roaming\Captain of Industry\Mods

The result should contain:
C:\Users\[User]\AppData\Roaming\Captain of Industry\Mods\KeyBound\manifest.json

Load a game, allow a few seconds for discovery, then find the generated binds
under Settings -> Mod keybinds -> KeyBound.

LIVE REBIND
-----------

Live Rebind lets you select a supported toolbar or calendar button and edit its
shortcuts without browsing the full Controls or Framework lists. It displays
the selected KeyBound bind, matching vanilla Controls bind when available, and
only the conflicts related to those shortcuts.

Activate Live Rebind from any of these places:

- The green Live Rebind shortcut in the Keybind Manager title bar.
- Shift-click or Ctrl-click the floating NEK button.
- Assign Toggle Live Rebind Mode under:
  KeyBound -> Misc. Binds -> KeyBound Binds.

While the mode is active, right-click or Shift/Ctrl-click a supported toolbar
or calendar button to select it. Choose Bind 1 or Bind 2, press the shortcut,
then select Update Keybinds. The Conflicts tab appears only when one of the
selected shortcuts conflicts with another active bind.

The K calendar button has its own KeyBound chooser for rebinding Open Bind
Controller, Open Keybind Manager, and Toggle Live Rebind Mode.

KEYBIND DISCOVERY
-----------------

Generated bindings are organized in Framework under:

- Vanilla: Menus & Tools
- Vanilla: Submenus
- Mod: Menus & Submenus
- Misc. Binds
- Vanilla: Items

KeyBound supports Bind 1, Bind 2, and left- or right-side modifiers. Assigned
shortcuts also appear in keybind labels when hovering supported toolbar menus.

The Vanilla: Items tab includes supported Storage, Transport, Vehicle,
Terraforming, Forestry, Surface, and Train items. Only researched content is
activated; the game's intentionally never-researchable dry trees remain usable.

The I/O Markers submenu is disabled by default. Enter add_markers_to_toolbar in
the game console to enable it.

KEYBIND MANAGER AND PROFILES
----------------------------

The Keybind Manager combines a saved vanilla .controls component with a saved
KeyBound .nek component in one .bindprofile. Applying that profile restores
both parts together.

First-time setup creates a protected Master Backup of the current vanilla
Controls before built-in or custom profiles are used. The Manager can then:

- Create a profile from current shortcuts or existing saved components.
- Rename, relink, overwrite, or delete Controls, NEK, and Bind Profile files.
- Overwrite one or both components already linked to a selected profile.
- Apply a selected profile or configure optional previous/next profile binds.
- Restore the protected vanilla Controls backup when needed.

Profile files are stored at:
C:\Users\[User]\AppData\Roaming\Captain of Industry\ModConfigs\not-enough-keybinds\Profiles

The legacy not-enough-keybinds data-folder name is retained so existing profile
components and backups remain available after the KeyBound rebrand.

CONTROLLER AND BUILT-IN PROFILES
--------------------------------

Left-click the K calendar button or floating NEK button to open the compact
Controller. Right-click either button to open the full Keybind Manager.

The Controller provides saved custom profiles and ready-made layouts including
Toolbar Mode, focused item profiles, Restore Controls, and Mag's All-In-One.
Built-in profiles keep the Toolbar Mode menu shortcuts unless Restore Controls
is selected.

The Controller and Manager title bars provide shortcuts between their related
windows, Live Rebind, settings, and soft-pin controls.

FLOATING NEK BUTTON
-------------------

The optional floating button is configured from the Keybind Manager's Settings
tab. It can be enabled, repositioned, locked, scaled, switched to text-only
mode, or changed between the included image designs.

LANGUAGES AND SETTINGS
----------------------

KeyBound includes English and partial translations for all 20 other supported
game languages: Catalan, Simplified Chinese, Traditional Chinese, Czech, Dutch,
Estonian, French, German, Hungarian, Italian, Japanese, Korean, Norwegian
Bokmål, Polish, Portuguese (Brazil), Russian, Spanish, Swedish, Turkish, and
Ukrainian.

The Use English UI option in the Manager's tooltip settings keeps KeyBound's
interface and tooltips in English without changing the language used by the
rest of the game.

The native Controller, Keybind Manager, and related UI are enabled by default.
Disable Enable Bind Manager in config.json to use only the Framework bindings.

NOTES
-----

- Menu shortcuts toggle their toolbar menu and preserve its last selected
  submenu. Submenu shortcuts open one exact submenu.
- Supported item and surface-tool shortcuts are suppressed while Settings is
  open so they do not activate while keybinds are being edited.
- KeyBound adds no save data and is safe to add to or remove from existing
  saves. Saved profiles remain in the ModConfigs folder unless removed manually.
