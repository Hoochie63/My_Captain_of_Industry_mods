ShipAutoExplore
===============

Automatically dispatches the ship to explore the nearest viable world map
node once it returns to port, refuels, and finishes repairs.

Installation
------------
1. Locate your COI data folder. By default:
   %APPDATA%\Captain of Industry

2. Open or create the "Mods" directory inside it.

3. Extract so you have:
   Mods\ShipAutoExplore\manifest.json
   Mods\ShipAutoExplore\ShipAutoExplore.dll

4. Launch the game, enable the mod via "Mods & DLCs", then load your save.

First-time activation
---------------------
Auto-explore is OFF by default on each new save to avoid surprise dispatches.
Enable it one of two ways:

  a) Open the ship inspector (click your fleet ship on the world map) and
     toggle "Auto-explore when idle" on. The ship will depart on the next
     game tick.

  b) Go to Options → Mods → ShipAutoExplore and set auto_explore_enabled
     to true. Changes take effect immediately — no reload required.

Configuration (Mod Settings in-game)
--------------------------------------
  auto_explore_enabled       — toggle auto-explore on/off (default: off)
  min_battle_score_margin    — ship score must exceed enemy score by this
                               amount before auto-dispatching (default: 10,
                               set to 0 to ignore battle score entirely)

How it works
------------
When the ship is docked, operational (fully crewed, health above minimum,
not repairing), and has an empty path, the mod searches all NotExplored
nodes for the nearest one that:

  1. Passes the game's own CanRequestLocationVisit() check — meaning fuel,
     crew, HP, and dock state are all acceptable.
  2. If the node's enemy strength is known, the ship's battle score exceeds
     the enemy score by at least min_battle_score_margin.

The nearest qualifying node (by roundtrip distance) is chosen and the ship
is dispatched automatically.

Tip: Enable "Automatically return home" in the ship panel so the ship
returns after each mission. This mod will dispatch it again once it docks,
refuels, and is ready.

Logs
----
%APPDATA%\Captain of Industry\Logs\
