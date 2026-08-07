Blueprint Products Info
=======================

Shows product flows and pollution for all buildings in a blueprint — directly in the
blueprint detail panel (right side of the blueprints window).

Features
--------
- Inputs — total consumption per minute for each product
- Outputs — total production per minute for each product
- Pollution — total air/water pollution per minute from recipe outputs
- Net flows — if the same product appears on both input and output, only the
  difference is shown (output minus input); zero net is hidden
- Product tooltips and section titles use the game's language (EN, RU, etc.)
- Right-click a product icon to open the codex

Usage
-----
1. Open the blueprints window.
2. Select a blueprint.
3. Scroll the detail panel — the mod adds sections below the vanilla content.

Rates are normalized to 60 seconds, the same way as machine recipes in the inspector.
Values from all buildings and assigned recipes in the blueprint are summed.

Notes
-----
- Only buildings with recipes (machines, generators, etc.) contribute to the totals.
- If a building has recipes assigned in the blueprint, those are used; otherwise all
  recipes of that building type are counted.
- Pollution is taken from recipe outputs marked as pollution (polluted air/water).
- Can be added to or removed from an existing save.

Requirements
------------
- Captain of Industry 0.8.6c or newer

Install
-------
Copy the mod folder to:
  %APPDATA%/Captain of Industry/Mods/BlueprintProductsInfo/

Or build from source (Release) — the project deploys automatically when
DeployToModsFolder is enabled in BlueprintProductsInfo.csproj.

Files
-----
- manifest.json, BlueprintProductsInfo.dll, 0Harmony.dll — required
- Thumbnail.png — mod icon in the game's mod list
- readme.txt — this file

Author: aneverse
