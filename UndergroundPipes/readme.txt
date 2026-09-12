Underground Pipes - a mod for Captain of Industry
Version 1.1.0 TEST  |  Verified on game 0.8.7a (game build 614)

WHAT IT DOES
- Route the game's normal fluid pipes, vanilla Pipe Connectors, and vanilla Pipe Balancers
  below the terrain and connect them back to surface networks. No replacement transport assets.
- Earned depth follows the vanilla pipe progression:
      T1 -> 1 layer   T2 -> 2 layers   T3 -> 3 layers   T4 -> 4 layers
  Depth is measured from the local terrain, including inside excavated mines below sea level.
- A direct native-style layer panel selects ALL, -1, -2, -3, or -4. It controls only the visible
  and selectable layer; the separate BUILD depth read-out always shows the actual placement depth.
- Network view switches between depth colours, product colours, and live diagnostics.
  Diagnostics marks moving, disconnected, blocked/idle, waiting, paused, low-power,
  and product-clearing pipes; click a hovered run to alternate between its destination and source.
- While digging, the route preview is colour-coded by validity:
  green = OK, amber = at the depth limit, red = invalid.
- Buried pipes are shown as a translucent, depth-colour-coded x-ray overlay.
  The default gradient runs from cyan at level 1 to orange at the deepest T4 level.
- The current depth level is shown at the cursor while you dig.
- Batched animated arrows sit directly on x-ray pipes. Direction follows the native trajectory,
  animation speed follows real moved-product steps, and stopped/empty/blocked networks become
  static diagnostic-coloured arrows. No GameObject or renderer is created per pipe segment.
- Placing a vanilla Pipe Connector on a buried pipe uses the game's own validation and cut-out
  transaction. The pipe is not changed during preview; confirmation splits exactly once and moves
  in-flight product through the native connector buffer without a temporary U-loop.
- Hover any pipe to see its product and amount or fill percentage at the cursor;
  the whole buried run lights up while you hover. Diagnostics view provides the
  network state separately.
- Product colouring, diagnostics, layer selection, demolition safety,
  colours, and brightness are available from the in-game mod settings.
- Buried pipes are protected from being deleted together with surface structures.

HOW TO USE
- Pick a pipe in the build menu as usual.
- Press the "lower" (down) button to dig the pipe below the surface, "raise" (up)
  to bring it back up. The level number is shown at the cursor.
- Manual routing still works: use lower/raise exactly as before. Each vanilla pipe tier
  unlocks one additional layer, up to four with T4.
- Layer toolbar panel: directly click ALL / -1 / -2 / -3 / -4. This is the VIEW/selection filter;
  use the normal up/down controls to change BUILD depth.
- Diagnostics toolbar button: cycle Depth / Product / Diagnostics views.
- Eye toolbar button: show / hide the underground x-ray overlay (and the hover read-out).
- Hover any pipe to see its current contents as a product icon at the cursor.
- Buried-pipe demolition is OFF by default. Enable it either with the trash toolbar
  button in the delete tool or in the mod settings only while removing buried pipes.
- In the delete tool, SAFE preserves the underground network. ALL + deletion enabled restores a
  normal mixed deletion. Selecting -1/-2/-3/-4 limits targets to that buried layer and protects
  surface buildings and every other underground layer; the highlighted refund preview is rebuilt
  from the exact targets that remain.
- Pipe Connector / Pipe Balancer: select the vanilla building, use down/up to choose a legal depth,
  then place normally. A connector can be positioned directly on a matching existing buried pipe.

INSTALL
- Extract the "UndergroundPipes" folder into:
      %APPDATA%\Captain of Industry\Mods\
- 0Harmony.dll is included and required.

DEVELOPMENT AND PACKAGING
- An ordinary `dotnet build -c Release` only builds the project; it never changes the
  installed game mod.
- To install a test build explicitly, set `COI_ROOT` to the Captain of Industry installation
  directory and use `dotnet build -c Release -p:DeployToGame=true`.
- To create and validate a clean release archive, run `scripts\New-ReleasePackage.ps1`.
  The script excludes debug symbols, validates versions/translations/package contents,
  creates a deterministic ZIP, and prints its SHA-256 checksum.

NOTES
- Underground Pipes is standalone. Cheat++ is not required. When both mods are installed, an optional
  compatibility guard prevents their height patches from conflicting without linking either mod.
  For fluid pipes managed by Underground Pipes, its tier and per-node support rules take priority;
  Cheat++ continues to control its own unrelated features.
- Burial depth is relative to the local terrain. Mines below sea level are supported;
  only the selected pipe tier's depth cap and the game's actual world bounds apply.
- A trajectory that genuinely enters the terrain is ground-supported without pillars. This avoids
  the game's false "cannot place supports" result on a building-port-to-tunnel transition. A fully
  surface route still uses the game's normal pillar and collapse rules.
- Pipe routes, endpoint snapping, turns and ramps are calculated by the game's native pathfinder,
  matching the proven 0.9.24 placement behaviour.
- Existing pipes deeper than the selected tier remain safe and visible, but their too-deep endpoint
  cannot be extended. Raise/rebuild that endpoint at a supported level before continuing the route.

CHANGELOG
1.1.0 TEST
- Fixed the Pipe Balancer's separate vanilla elevation and ground-vertex validators reporting
  "Terrain too high" for every legal underground placement. Runtime terrain callbacks also ignore
  buried junction ground constraints so a valid build cannot immediately enter the collapse path.
  Surface placement and all non-terrain validators stay native.
- Added underground construction for the vanilla Pipe Connector and Pipe Balancer. They use native
  entity serialization, ports, buffers, inspectors, unlocks and construction commands, so existing
  saves need no migration and other mods remain optional.
- Extended the native MiniZipper connector transaction below terrain: preview remains read-only;
  confirmation validates and cuts the selected straight segment once, preserving product and flow
  direction without synthesizing a U-shaped pipe route or pre-blocking the new ports.
- Added a direct ALL/-1/-2/-3/-4 layer panel to pipe, connector/balancer and demolition tools. The
  cursor explicitly labels BUILD depth and VIEW filter as separate values.
- Added cached, batched x-ray flow arrows driven by Transport.MovedStepsTotal. Moving arrows inherit
  Depth/Product/Diagnostics colours and proportional speed; empty, blocked, disconnected, paused,
  low-power and clearing runs stop and use clear diagnostic colours.
- Added x-ray copies for buried native connectors and balancers, without per-segment Unity objects.
- Made buried connector/balancer x-ray models directly selectable. Clicking their visible footprint,
  including the empty center of a Pipe Balancer, now opens the entity's native configuration inspector
  without requiring a terrain raycast.
- Fixed false "Collision with terrain" errors when joining a pipe to a buried connector/balancer port.
  Tier depth is validated along the pipe axis and local terrain instead of the ramp's lower clearance
  envelope, while command-side and pathfinder depth limits remain enforced.
- Hardened demolition filtering: a selected layer excludes surface entities and every other buried
  depth from both the exact preview/refund calculation and command execution. SAFE preserves the
  underground network; ALL plus explicit demolition permission allows a normal mixed removal.
- Kept every new UI/overlay patch isolated with bounded retry logging, so a renderer or private-UI
  signature failure cannot disable vanilla transport construction.
- Verified and built against Captain of Industry 0.8.7a (game build 614). Public Stable status,
  publishing state and tags are intentionally unchanged pending in-game user verification.

1.0.6
- Restored native surface-only routing: a normal surface pipe can no longer silently tunnel
  through a hill. Underground collision forgiveness is enabled only for a route that actually
  starts from, is aimed at, or has its selected path height below the local terrain.
- Fixed U-shaped loops on direct building connections. A building port itself is at surface height,
  so underground intent is now read from the pathfinder's selected absolute height instead of being
  inferred only from the two surface-level endpoints. Pillar pathfinding is skipped only for that
  explicit underground request; fully surface routing keeps the native game setting.
- Added a staged straight riser for deep T2-T4 building connections. If the native pathfinder leaves
  the selected underground layer too late and creates a final U-turn, only that endpoint detour is
  replaced with a validated straight profile that rises one layer at a time. Native obstacle routing,
  surface pipes and already-direct ramps are left unchanged.
- Enforced T1=-1, T2=-2, T3=-3, and T4=-4 against the local terrain throughout pathfinding,
  final trajectory validation, and command execution, including hills and excavated terrain.
- Restored the proven buried-route support rule from 0.9.24/1.0.5: a route that enters terrain no
  longer asks for an impossible pillar below a building port. Fully surface routes remain vanilla.
  Existing legacy pipes deeper than today's tier cap remain supported and are not destroyed merely
  because an old save is loaded.
- Isolated routing and runtime support from Cheat++'s underground mode. Neither mod is required by
  the other, and Underground Pipes no longer reads or changes Cheat++'s construction mode.
- Reset all session-owned overlays, caches, managers, toolbar state, and inspector references when
  leaving or loading a game, preventing stale x-rays, duplicate controls, and cross-save state.
- Fixed stale/incorrect inspector icons and made diagnostics endpoint clicks work even when the
  hover read-out is hidden. UI clicks no longer trigger a camera jump to a pipe behind the window.
- Endpoint navigation no longer guesses an arbitrary branch on balancers and distributors; an
  ambiguous network stops at the branching device instead of showing the wrong building.
- Reduced large-network overhead with positive and negative terrain caches, slower terrain-signature
  refreshes, in-place port updates, debounced settings refreshes, and real error backoff.
- Hardened interactive overlays so a rendering failure cannot abort native pipe construction or
  deletion input; the first failure now includes a useful stack trace without flooding the log.
- Build, live-game deployment, and release packaging are now separate operations. Ordinary builds
  no longer overwrite the installed mod; test deployment requires an explicit build property.
- Updated the bundled Harmony dependency to 2.4.2 through a restorable NuGet PackageReference.
- Release builds use deterministic source paths and no longer publish a local-path PDB.
- Added a clean, validated, deterministic packaging script with version, translation, allow-list,
  and checksum checks.
- Corrected the readme and config descriptions for the current hover read-out and depth gradient.
- Completed the 1.0 interface keys in every shipped translation file.

1.0.5
- Restored the proven 0.9.24 placement core for normal underground-to-surface connections.
- Removed the experimental route assistant, underground junction snap, and temporary slope override
  from the build-input path. They could alter the destination cursor or run outside the game's
  asynchronous pathfinding phase, producing detours and U-shaped loops near ports.
- Ordinary terrain again uses the same placement floor as 0.9.24. Low excavated terrain gets only
  the additional local allowance needed to retain the selected tier depth and mine support.
- T1=-1, T2=-2, T3=-3, and T4=-4 remain enforced independently of Cheat++.
- Layer filters, x-ray colours, product/diagnostic views, hover inspection, and safe demolition remain.

1.0.4
- Attempted to shorten underground-to-port ramps with a temporary one-tile vertical step. The game's
  path calculation is asynchronous, so the override did not apply at the correct phase and was removed
  in 1.0.5 instead of changing native routing further.
- When both mods are installed, an optional guard replaces only Cheat++'s conflicting unlimited-height
  result with Underground Pipes' finite tier depth limit; neither mod is a dependency of the other.
- Buried routes now skip the irrelevant vanilla pillar search before it can emit a false support warning.

1.0.3
- Fixed "cannot place supports" when starting an underground pipe directly from a building port.
- Restored the proven no-pillar rule for routes that genuinely enter the terrain; fully surface
  routes still use the game's normal pillar rules.

1.0.2
- Fixed an integer overflow when Cheat++ U mode and Underground Pipes were active together. The
  overflow sent a new pipe vertically toward height 2147483647 instead of keeping it at its tier.
- Underground Pipes detects and safely replaces Cheat++'s unlimited-height result while retaining
  all other Cheat++ features.
- The x-ray visibility toggle now resets to ON for each loaded game instead of carrying an accidental
  hidden state from a previous save in the same process.

1.0.1
- Underground Pipes now keeps the T1=-1, T2=-2, T3=-3, and T4=-4 depth limits even
  when Cheat++ underground construction is enabled.
- Added continuous tier-depth clamping so an out-of-range cursor from another mod is
  corrected before and after the game's build input update.

1.0.0
- Added opt-in smart surface-to-tunnel-to-surface route profiles.
- Added x-ray-aware snapping to existing buried pipes and native inline junction creation.
- Added layer filtering: All, -1, -2, -3, and -4+; hover and connection lookup respect it.
- The layer-filter toolbar button now displays its live mode directly on the button: ALL, -1, -2,
  -3, or -4+.
- Added Depth, Product, and Diagnostics network views. Diagnostics uses the pipe's real
  simulation status and lets a click alternate between destination and source buildings.
- Expanded balanced depth progression to all vanilla pipe tiers: T1=1 through T4=4.
- Added explicit English and Russian UI text for all 1.0 controls and status labels.
- Recolouring a live network no longer rebuilds its geometry, reducing diagnostics overhead.
- Road compatibility now reuses per-thread pathfinding buffers instead of allocating on every retry.

0.9.25
- Verified and rebuilt for Captain of Industry 0.8.7a.
- Fixed routing in excavated mines below sea level: minimum placement height is now
  calculated from the local terrain instead of being clamped to absolute sea level.
- Fixed missing supports on exposed spans over valleys. Only genuinely buried nodes
  bypass pillar and terrain checks; surface sections retain the vanilla rules.
- Fixed mixed-elevation x-ray colouring: each rendered section now uses its own local
  depth instead of inheriting the deepest point of the whole route.
- Added a clearly labelled demolition-safety switch and warning in the settings window.
- Language files now reload correctly after changing language or loading another save,
  and JSON Unicode escapes are decoded correctly.
- Harmony patches are applied independently, so one game API change disables only the
  affected feature instead of preventing the whole mod from loading.
- Fixed cached x-ray materials leaking across live settings changes.
- Added the Harmony license to the package and aligned manifest metadata with CoI 0.8.7.

0.9.24
- Fix for game 0.8.6a: reloading a save could make unlocked research, recipes, and most build-menu
  categories appear to be missing. The mod's persistent HUD watchdog was resolving UI services while
  the game's properties database was still being restored from the save. It is now suspended during
  scene unload/load, and all runtime UI dependencies are initialized only after loading has completed.
- Existing saves are not expected to have lost their research data; loading them with this version
  should restore the normal UI without needing to resave or start a new game.

0.9.23
- Improved the Russian translation to better match the game's own style and terminology
  (contributed by Hoochie). Thanks!

0.9.22
- New: the mod's UI (settings window, toolbar tooltips, pipe inspector, hover read-out) is now
  localizable and ships with translations for 12 languages besides English: ru, uk, de, fr, es,
  pt_BR, pl, cs, it, zh_Hans, ja, ko. The game's active language is picked up automatically; an
  unsupported language just stays English.
- For translators: to add or fix a language, copy translations/en.json to translations/<code>.json
  (the <code> matches the game's own translation file names, e.g. ru, de, zh_Hans) and translate the
  text after each colon. No code changes needed - send me the file and I'll include it.

0.9.21
- Fix: buried pipes could not be routed UNDER roads from the "BeTTerLife: Roads and Signs" mod
  (reported by Planb4u) - its road pieces invisibly occupy 2 tiles (edges: up to 25) BELOW the
  surface, so no burial depth was legal under a road. Buried pipes now ignore road-entity occupancy
  on tiles fully below the surface; everything else is untouched: pipes still can't clip through
  the roadbed at grade, and digging/building under roads stays forbidden for everything else.

0.9.20
- Fix: the settings gear button could be MISSING entirely when another HUD-modding mod (reported
  with COIE) rebuilds the calendar bar after our button was placed: rebuilding detaches the button
  from the UI, and the old watchdog could only re-SHOW a hidden button, not resurrect a destroyed
  one. The watchdog now detects that the button is no longer attached and re-places it (every ~2s).
- Change: the gear now prefers the top DATE/WEATHER row (next to the menu button, where other mods'
  buttons live) instead of the game-speed row, which speed-modding mods hide or rebuild wholesale.
  If the date row can't be found, it falls back to the old busiest-row placement.
- Fix: a failed placement attempt (e.g. another mod mid-rebuild of the HUD tree) no longer gives up
  for the whole session - it retries every ~5 seconds (up to 20 attempts).

0.9.19
- Fix: the buried-pipe x-ray (and the hover read-out) was INVISIBLE after the first save load of a
  freshly started game, until you clicked any build tool (reported by warrenc). An integer overflow
  in the "is a build tool active" check made the always-on x-ray permanently yield to a tool that
  was never open. One-line fix.

0.9.18
- Fix: the "from -> to" block appeared in the inspector of CONVEYOR BELTS (and molten channels) too -
  it now shows only for fluid pipes (the game uses one shared inspector for all transports, so the
  block needed an explicit pipe check).
- Fix: the "from -> to" block could appear TWICE (or more) in one inspector after loading a save:
  the mod re-applied its Harmony patches on every save load within one game process, stacking a
  duplicate panel each time. Patches are now applied exactly once per process.

0.9.17
- Fix: the settings gear button could be INVISIBLE even after being placed, when the Speed++ mod is
  installed: Speed++ clears out the vanilla speed buttons by hiding everything in that row whose
  component type is a plain ButtonIcon, and our gear matched that filter. The gear is now a dedicated
  button type (immune to that sweep, and the same component class every other visible mod button in
  that row uses), and a watchdog re-shows it every couple of seconds in case any other mod hides it.

0.9.16
- Fix: the settings gear button (in the top game-speed bar) did not appear when loading a saved game.
  Three causes, all fixed: (1) it resolved the HUD eagerly in EarlyInit, which fails on a save load -
  now it resolves lazily with retry; (2) the button was tied to the settings window building - now the
  button is placed independently and the window builds on first click; (3) the per-frame tick that
  places the button shared one try/catch with the hover read-out and inspector, so if either of those
  threw for a frame the button code never ran - each now has its own try/catch.

0.9.15
- The pipe inspector is now EMBEDDED into the game's own pipe panel: click any pipe (buried or
  surface) and the native panel gets a "source -> destination" section showing the real building
  that feeds the pipe and the one it feeds (clickable to fly the camera there), plus how full those
  tanks are. The separate floating inspector window is gone - it's all native now.

0.9.14
- Source/destination now traces through ALL routing devices (connectors, balancers, zippers,
  mini-zippers, distributors) and stops only at a real machine or storage tank - so it shows the
  actual building, not the connector in between.
- Turning off "Source/destination building icons" now removes them completely instead of showing
  crossed-out placeholder icons.
- Removed the fill bar from the cursor read-out (the "P%" number already shows how full the pipe is).
- Settings window tidied up: more spacing between rows, sections indented, and the brightness slider
  now shows the real percentage (it used to show the track position).

0.9.13
- X-ray brightness is now a SLIDER (drag it and the whole overlay dims/brightens live), with the
  current % shown next to it. Default brightness lowered to 40% so the pipes sit more in the
  background out of the box.

0.9.12
- Added a global x-ray BRIGHTNESS control (requested on the forum): an "X-ray brightness" section
  in the settings with preset buttons (20-100%) that dims/brightens ALL buried pipes at once.
  Also editable via config.json (overlayBrightness). Lower it if the pipes look too bright.

0.9.11
- The settings panel is now a proper game window (like Cheat++): a title bar with a close (X)
  button and a pin, and you can drag it around. Section headers, and the colour schemes are shown
  as bordered two-colour "chips" so it's clear each one is a scheme you can click.

0.9.10
- The tank read-out now shows the actual amount as well as the percentage, e.g.
  "in 4/400 (1%)  ->  out 320/400 (80%)".

0.9.9
- Fixed tank fill always reading 0%: it is now computed from the tank's real current/capacity
  (the raw Percent value it used before was not a 0-100 percentage).
- The inspector is now auto-pinned to the LEFT edge of the screen, vertically centred - no more
  position sliders (they wouldn't drag). One less thing to fiddle with.

0.9.8
- Removed the "flowing/stuck" word from the cursor read-out (it jittered) - the fill bar already
  shows how full the pipe is.
- The inspector now shows storage TANK fill %: if the pipe comes from and/or goes to a tank, it
  reads "in 73%  ->  out 20%". The trace now stops at tanks/storages (so the tank is shown as the
  source/destination) while still passing through pipe balancers/sorters.
- The inspector now defaults to a fixed spot at the top-left, just under the top bar (turn on
  inspectorAtCursor to have it pop up where you click instead).
- New compact in-game settings panel: real checkboxes, LIVE sliders for the inspector position and
  icon size (drag and it moves in real time), and clickable colour-scheme swatches for the x-ray
  colours - all in a scrollable box that fits the screen.
- Pipe length shows a clean whole number of tiles.

0.9.7
- Settings gear is now slotted INTO the top game-speed bar (in line with pause/play/fast-
  forward), instead of floating over the world where it overlapped the controls.
- Settings panel now fits any screen: it is a fixed-size, SCROLLABLE box.
- The clickable inspector now appears right where you click the pipe (set inspectorAtCursor
  to off to pin it at a fixed InspectorTop/InspectorLeft instead).
- Fixed the ugly pipe length: it now shows a rounded whole number of tiles (was e.g. 8.608398).

0.9.6
- Read-out no longer jitters: the "flowing/stuck" word and numbers sit in their own fixed
  boxes and only update when they actually change.
- Source/destination now traces THROUGH routing buildings (pipe balancers, distributors,
  sorters, valves - e.g. the Gameplay++ Pipe Balancer/Sorter) to the real machine that
  produces or consumes the fluid, instead of stopping at the balancer.
- The clickable inspector is now driven by CLICKING a pipe (not hovering), so you can safely
  move the mouse to it without changing the selection. It stays visible, is OFF by default,
  and now shows richer info: product name, buffered amount + fill %, pipe length, and live
  flow (units moved per second) / stuck. Its two building icons still fly the camera there.
- Settings gear moved to the top-right, near the game-speed controls (position configurable
  via settingsButtonTop / settingsButtonRight) instead of the screen centre.
- InspectorTop / InspectorLeft now move the panel live when changed.
- MaxDepth is no longer an editable setting (2 levels for T2+ pipes is a fixed mod feature).

0.9.5
- Source/destination now shows the REAL building: the read-out walks THROUGH pipes, lifts and
  routing buildings (distributors, valves, junctions) and stops at the actual machine that
  produces or consumes the fluid, instead of stopping at a distributor in the middle.
- Cleaner panel layout: everything is vertically centred with even spacing and consistent icon
  sizes (previously the icons sat crooked next to the text and bar).
- The clickable inspector panel now stays visible permanently (it used to vanish after a few
  seconds). It is OFF by default - turn it on in the settings.
- NEW in-game settings button: a gear icon just under the top time/speed controls opens a panel
  with ALL of this mod's settings (toggles + fields), so you no longer have to hunt for them.
  (The settings also still live under Main Menu -> Mods & DLCs -> the gear on this mod's tile.)

0.9.4
- NEW clickable "pipe inspector" panel (a small fixed panel in the corner): it mirrors the
  last pipe you hovered - source building -> product -> destination building - and the two
  building icons are CLICKABLE: click one to fly the camera straight to that building.
  (The cursor read-out follows the mouse and can't be clicked; this panel sits still and
  stays up for a few seconds after you stop hovering so you can reach it and click.)
- New settings:
    showInspector           - turn the clickable inspector panel on/off
    inspectorTop/Left       - where the inspector sits on screen (pixels)
    inspectorLingerSeconds  - how long it stays clickable after you stop hovering

0.9.3
- Hover read-out now shows the SOURCE building (what feeds the pipe) and the DESTINATION
  building (what it feeds) as icons on either side of the product - it walks through
  connected pipes/junctions to find the real building at each end.
- The source/destination building icons are now larger and their size is configurable.
- New settings (config.json / in-game mod settings):
    showReadout        - turn the whole hover read-out on/off
    showBuildingIcons  - turn the source/destination building icons on/off
    iconSize           - size of those icons in pixels
    colorShallow       - x-ray overlay colour for the shallowest level (hex, e.g. 00d2ff)
    colorDeep          - x-ray overlay colour for the deepest level (hex, e.g. ff7700)

0.9.2
- Fix: the hover read-out now shows the real unit amount in a pipe (matching the game's
  own inspector) instead of the number of product packets - it used to read half the
  amount because each packet can carry more than one unit.

0.9.1
- Cheat++ compatibility: buried pipes stay protected from bulldozing by default, and this
  mod's "delete buried pipes" button now also flips Cheat++'s matching toggle - so one button
  frees both mods at once instead of the old "you must enable it in both" conflict. When
  Cheat++'s own burying is active, this mod yields the depth stepping to it to avoid a double
  step. The x-ray overlay and hover read-out keep working on top of whichever mod buried the pipes.

0.9.0
- The "delete buried pipes" toggle now always starts OFF when you load a game, so a
  toggle left on in a previous session can't accidentally bulldoze buried pipes.
- Hover read-out v2: the cursor panel now also shows a fill bar and a flowing/stuck
  indicator, and hovering lights up the whole buried run in the x-ray overlay.
- Configurable settings (config.json / in-game mod settings): maximum burial depth
  (1-8) and an optional "colour by product" overlay mode.
- Colour-by-product: option to tint the x-ray by the product in each pipe instead
  of by depth, so you can tell networks apart at a glance.

0.8.1
- Fix: building/connecting a buried pipe could throw "Set changed while enumerating"
  in the overlay and abort the toolbar input update, which could break the connection
  being made. The overlay now iterates a safe snapshot.
- Fix: hover read-out no longer spams an assert when the cursor is over off-map tiles.

0.8.0
- Tier-based depth: Tier 1 pipe digs 1 level down, Tier 2+ pipes dig 2 levels.
  Deeper burial is now earned through pipe-tier research instead of being free.
- Placement preview is colour-coded by validity (green / amber / red).
- Hover read-out: point at any pipe (buried or surface) to see what is flowing
  through it as a big product icon + count at the cursor.
- Toolbar buttons line up flush with the rest of the bar; stability fixes.

0.5.0
- Route vanilla fluid pipes underground with a depth-colour-coded x-ray overlay,
  a show/hide button, and protection from accidental deletion.

CREDITS
- Built with Harmony (0Harmony.dll, MIT license - see licenses/0Harmony-LICENSE.txt).
