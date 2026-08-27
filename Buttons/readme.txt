BUTTONS v1.0.0

Buttons keeps Captain of Industry's calendar HUD clean by letting you choose
which native and mod-added controls are visible, where they appear, and when
they are revealed.

FEATURES

* Discover and manage native and mod-added calendar HUD buttons.
* Reorder buttons while preserving their original actions, icons, tooltips,
  labels, and live state changes.
* Configure independent Primary and Secondary views.
* Switch views by clicking the Buttons control or reveal Secondary on hover.
* Choose left or right expansion and adjust the On Hover delay.
* Enable, hide, recolor, and bulk-edit buttons with one-session bulk Undo.
* Use preset colors, RGB sliders, or hexadecimal color values.
* Use Speed / Mod Mode to group speed controls separately from mod buttons.
* Resize and pin the settings window, disable tooltips, refresh discovery,
  force English UI text, or restore defaults.

PROFILES

Buttons uses one global configuration across every world and game session.
Named profiles can save and restore the complete configuration at runtime.
Profiles remain in AppData when the installed mod folder is removed.

When Tweaks++ is available, every profile also records the calendar HUD
position. The unlabeled Include position checkbox beside Load decides whether
that saved position is restored; normal startup and world loading are unchanged.

OPTIONAL TWEAKS++ CONTROLS

When a compatible Tweaks++ installation is detected, the Buttons title bar adds
a shortcut to the Tweaks++ HUD page and a one-click lock/unlock shortcut for HUD
positions. Both controls disappear when their compatible Tweaks++ targets are
unavailable.

USING THE BUTTONS CONTROL

* Revealable buttons off: click to open settings.
* On Click: left-click switches Primary and Secondary; right-click opens
  settings.
* On Hover: hovering reveals Secondary; clicking opens settings.

On Click always keeps the Buttons control available. On Hover may hide it from
Primary when another Primary button remains visible, while Secondary keeps
settings access available.

INSTALLATION

Extract the single Buttons folder into:

%APPDATA%\Captain of Industry\Mods

The installed folder must contain manifest.json and SexyCalendar.dll directly.
Remove or disable any older version that manages the same calendar HUD row
before loading Buttons.

COMPATIBILITY

* Captain of Industry 0.8.6c through verified 0.8.7b.
* Safe to add to or remove from an existing save.
* No required mod dependencies.
* Speed++ and Tweaks++ integrations are optional.

PACKAGE IDENTITY

* Display name: Buttons
* Author: Underlörd
* Mod ID, installation folder, and ZIP root: Buttons
* Runtime DLL: SexyCalendar.dll
