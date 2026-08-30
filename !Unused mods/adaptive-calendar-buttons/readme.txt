ADAPTIVE CALENDAR BUTTONS v1.0.8

Adaptive Calendar Buttons keeps Captain of Industry Calendar 1 buttons compact,
sortable, and under your control while preserving their original actions,
icons, live behavior, and reliable metadata.

ONE ADAPTIVE BUTTON

The mod adds one calendar button instead of separate settings and view controls.

* Revealable buttons off: left-click opens Adaptive Calendar Buttons - Settings.
  Right-click has no action.
* On Click mode: left-click switches between the configured Primary and
  Secondary views.
  Right-click opens settings.
* On Hover mode: either click opens settings. Moving the pointer over the
  calendar temporarily switches from Primary to Secondary; leaving restores
  Primary.
* The control icon has separate selectable colors for Primary and Secondary.
  Defaults are white and gold respectively.
* Its translated tooltip always explains the currently available clicks.
* On Click always keeps the Adaptive Calendar Buttons control available in both
  views.
* On Hover can hide the control from Primary while another enabled Primary
  button remains available. Secondary always restores the control.
* Primary can never become empty. Clearing all ordinary Primary or Enabled
  buttons restores the calendar control so settings remain accessible.

ENABLED AND TWO INDEPENDENT VIEWS

Every eligible calendar button has direct shared-page controls:

* Enabled is the master switch. Disabled buttons are always hidden.
* Primary and Secondary choose click-switched visibility in On Click mode.
* Primary and Secondary choose temporary hover-switched visibility in On Hover
  mode.
* Color optionally tints that button's live icon. Original preserves native or
  mod-provided colors and state changes.

This supports every useful combination: visible in both views, visible in only
one view, or completely disabled. Turning Enabled off preserves both view
choices and the selected icon color for later.

The settings window includes a native pin control beside Close. Pinning is
soft: Escape skips only this pinned window and remains available to close any
other unpinned window.

The fixed bottom row contains independent bulk controls for Enabled, both view
columns, and icon color. Each changes only its own column. After a bulk change,
Undo last bulk change restores the exact previous mixture for the current
session. In On Hover, turning every ordinary Primary or Enabled button off
keeps the calendar control as the safe remaining button.

ORDER AND EXPANSION DIRECTION

The settings list always reads in the same left-to-right order as the live
calendar bar. Switching between Expand left and Expand right reverses both the
live row and the list together, so familiar sequences such as pause, speed 1,
speed 2, and speed 3 never require mental mirroring.

Select a green-highlighted row and use the compact arrow buttons to reorder it.
Moving a row up always moves that live button left. Moving it down always moves
the button right, regardless of expansion direction. Selection updates in
place and the scrolling list retains its position.

Expand left is the default for the common top-right calendar layout. When
Tweaks++ has repositioned the calendar, Adaptive Calendar Buttons keeps the right
edge anchored so width changes grow naturally toward the left. Expand right
uses the normal fixed left edge.

Direction controls are shown only while revealable buttons are enabled and a valid
Tweaks++ movement target is available. The untouched native calendar already
positions itself correctly. During a real Tweaks++ drag, the right-edge
correction is suspended and then adopts the completed position. Adaptive
Calendar Buttons preserves the visible placement and vertical coordinate while
synchronizing the corrected horizontal coordinate through Tweaks++ so reloads
remain stable.

RESET TO DEFAULTS

Reset to defaults uses the game's native confirmation prompt. It restores:

* The original discovered visual order.
* Default button visibility, including the Speed++ conditional defaults.
* Revealable buttons off.
* On Click as the interaction mode and Primary as the remembered active view.
* Expand left as the default direction.

Reset never moves the calendar on screen or changes the settings-window position.
When needed, only Tweaks++'s stored horizontal coordinate is synchronized to the
calendar position already shown on screen.

SPEED / MOD MODE

Speed / Mod Mode maintains a layout that adapts to the selected interaction
mode and expansion direction:

* On Click keeps the split layout: pause and speed controls appear in Primary,
  while every other discovered button appears in Secondary.
* On Hover keeps pause and speed controls visible in Primary, then reveals every
  enabled calendar button in Secondary while the pointer is over the calendar.
* The Speed++ live speed display is grouped with the speed controls whenever it
  is discovered.
* Expanding left puts mod buttons before the speed block so the At Rest speed
  controls remain at the same screen position when the other buttons appear.
  Expanding right retains the speed-first visual order.
* Manually changing button order or visibility disables Speed / Mod Mode so
  custom layouts remain under user control.

GLOBAL SETTINGS

One global configuration is shared by every world and survives complete game
restarts. It remembers order, Enabled, both view assignments, interaction mode,
expansion direction, per-button icon colors, and both control-state colors. The
active On Click view is also remembered, so Primary or Secondary resumes after
loading. On Hover always begins safely in Primary / At Rest and remembers
whether its calendar control is included there.

There are no per-save profiles or multiple-profile setup in v1.0.8. New test
worlds automatically use the same global configuration.

SPEED++ DEFAULTS

Speed++ is optional. On a fresh configuration with Speed++ installed, the
native pause and speed controls it replaces default hidden while its current
speed display remains available. Without Speed++, native pause and speed
controls default shown. Existing saved choices remain authoritative until
Reset to defaults is confirmed.

DISCOVERY, NAMES, AND TOOLTIPS

* Detects native and mod-added calendar buttons from the live HUD.
* Preserves each discovered button's original icon, action, tooltip metadata,
  translation, and live behavior whenever its source provides them.
* Uses reliable mod display names when available. Internal structural labels
  such as Wrapper fall back to the owning loaded mod's manifest display name.
* Recognizes native pause and game-speed controls with translated names.
* Lists Mori's supported control as Mori++ Overlord Menu while leaving its
  original action and dynamically sourced tooltip untouched.
* Genuine third-party name/icon cells expose the original tooltip supplied by
  that mod. Live calendar tooltips are never altered.
* Individual row checkboxes contain no repeated tooltip clutter. Focused help
  remains on global controls, column headers, Refresh, Reset, and reorder
  arrows.

Refresh performs one immediate re-scan for buttons added, removed, or changed
by other mods without resetting saved settings. Stable scans avoid reinserting
already-correct controls, preventing repeated hover and tooltip flashing.

Stable identities and collision safeguards keep saved state attached to the
correct controls. Temporarily late-loading buttons retain their records;
genuinely removed dynamic buttons are forgotten only after a long absence
grace period.

LANGUAGES

Includes English, Catalan, Czech, German, Spanish, Estonian, French, Hungarian,
Italian, Japanese, Korean, Norwegian, Dutch, Polish, Portuguese, Russian,
Swedish, Turkish, Ukrainian, Simplified Chinese, and Traditional Chinese.

Enabled, Primary, Secondary, Color, and Calendar Button intentionally remain compact
English headers so translated text cannot break the tightly aligned table.
Their full explanations remain translated tooltips. Metadata supplied by other
mods is not retranslated.

LAYOUT AND COMPATIBILITY

* Uses the verified CalendarControlsHud structure directly.
* Managed buttons and the Adaptive Calendar Buttons control remain in the native
  Calendar 1 speed row.
* The Captain's Hat Escape menu remains untouched in native Calendar 2.
* Includes no docking mode or Calendar 2 placement controls.
* Compatible with Captain of Industry 0.8.6c through verified 0.8.7a.
* Safe to add to or remove from an existing save.
* No required mod dependencies.

OFFICIAL PACKAGE IDENTITY

* Display name: Adaptive Calendar Buttons
* Mod ID and installation folder: adaptive-calendar-buttons
* Public ZIP: Adaptive Calendar Buttons v1.0.8.zip
* ZIP root folder: adaptive-calendar-buttons
* Runtime DLL: SexyCalendar.dll
