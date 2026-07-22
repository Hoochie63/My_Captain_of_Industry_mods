Production Calculator
=====================

Plan production chains by defining product rows manually and seeing required
buildings, net imports, and net exports for the full chain.

Features
--------
- Multiple product rows with individual rates (/min) and per-row machine counts
- Input / Output flow mode per row (anchor rate on recipe input or output)
- Recipe selection per row when a product has more than one recipe
- Live chain calculation (updates automatically as you edit rows)
- Sync chain rates: propagate rates through linked rows
- Fixed toggle: keep a row's rate and machine count when syncing
- Auto-suggested rates when picking a product already present in the chain
- Required buildings per recipe (machine count)
- Required inputs per minute (net import into the chain)
- Target recipe outputs per minute (net export from the chain)
- Save / Load calculation presets (name + icon) to JSON files in the mod folder
- Split UI: scrollable product chain on the left, results panel on the right

Usage
-----
1. Open the calculator via the Stats toolbar button or press F10.
2. For each row, choose Input or Output, pick a product and recipe, set rate
   and machine count.
3. Click "Add product" to add more rows and build the chain manually.
4. Use "Sync chain rates" to fill row rates from the current calculation.
   Enable "Fixed" on rows you do not want changed by sync.
5. Review results on the right: buildings, net inputs, net outputs.
6. Use Save / Load (bottom of the right panel) to store or restore presets.

Save / Load
-----------
- Presets are stored as JSON files in:
    {ModFolder}/SavedCalculations/
- They are not written into the game save.
- After loading a preset, Save pre-fills its name and icon for easy overwrite.

Notes
-----
- Each row owns its recipe choice; rows that share a recipe contribute to one
  machine count for that recipe (the highest demand wins).
- Inputs show net demand: total consumption minus in-chain production.
- Outputs show net surplus: total production minus in-chain consumption.
- Small rounding differences between rows are filtered out in net totals.
- Rates are normalized to 60 seconds, matching the in-game recipe inspector.
- Can be added to or removed from an existing save.

Requirements
------------
- Captain of Industry 0.8.6a or newer

Install
-------
Copy the mod folder to:
  %APPDATA%/Captain of Industry/Mods/ProductionCalculator/

Or build from source (Release) — the project deploys automatically when
DeployToModsFolder is enabled in ProductionCalculator.csproj.

Author: aneverse
