# Grid Battery

Grid Battery 1.0.2 adds three upgradeable industrial electricity-storage buildings to Captain of Industry. Each tier has independent charging and generation priorities, surplus-power controls, persistent stored energy, load-dependent maintenance, and a dedicated optimized model.

## Features

- Lead-acid T1, Li-ion T2, and solid-state T3 batteries.
- 100 / 400 / 1,600 MWs capacity.
- 250 kW / 1 MW / 4 MW charging power.
- 1 / 4 / 12 MW discharging power.
- 80% / 90% / 94% charging efficiency.
- In-place upgrades that preserve settings and stored energy.
- Separate consumption and generation priorities.
- Support for `Use surplus power only` and `Allow surplus power`.
- Maintenance I for T1 and Maintenance II for T2/T3.
- Tier unlocks and unlimited +2% capacity and power research.
- No workers required; every tier occupies 5x4 tiles.

## Construction costs

- T1: 40 Construction Parts II, 10 Electronics, 10 Steel.
- T2: 60 Construction Parts III, 20 Electronics II, 20 Steel.
- T3: 80 Construction Parts IV, 30 Electronics III, 40 Glass.

## Installation

Extract the archive so the resulting directory is `%APPDATA%\Captain of Industry\Mods\GridBattery`. Enable Grid Battery when creating or loading a game. Back up important saves first: the mod can be added to an existing save, but must not be removed after battery entities have been saved.

## Localization

Version 1.0.2 includes English, Russian, and 31 additional localizations. Translation catalogs are read from editable JSON files in the mod's `Localization` directory and selected from the active Captain of Industry culture when prototypes are registered. Unknown or unavailable cultures fall back to the external English catalog; restart or reload the game after changing its language.

## Compatibility

Grid Battery 1.0.2 targets Captain of Industry 0.8.7 and game API assemblies 0.8.7.0. Compatibility with later game updates is not guaranteed.

## License

Author: LordXaosa. Source code is licensed under the MIT License. Original models, textures, icons, previews, and other visual assets are licensed under CC BY 4.0 with attribution.
