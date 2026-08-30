Grid Battery 1.0.7

Three upgradeable electrical storage buildings with separate charging and discharging priorities plus vanilla surplus, pause, copy-settings, upgrade, deconstruction, and load-dependent maintenance behavior.

Version 1.0.7 uses Captain of Industry's vanilla priority queues for charging and consumes partial allocations immediately, keeps surplus-only charging exclusive to surplus generators, shows priority 12 on both controls for new batteries, and prevents full-charge oscillation at equal or lower generation priority.

Installation
1. Close Captain of Industry.
2. Copy the GridBattery directory from this release to `%APPDATA%\Captain of Industry\Mods\GridBattery`.
3. The directory must contain manifest.json, GridBattery.dll, readme.txt, README.md, changelog.txt, the Localization directory, AssetBundles\mafi_bundles.manifest, and the extensionless AssetBundles\gridbattery_models_* runtime bundle.
4. Start the game and enable Grid Battery when creating or loading a game.

Compatibility and saves
This release targets and was tested with Captain of Industry 0.8.7 (assemblies 0.8.7.0). It is not verified for any broader game-version range; game updates can change experimental mod APIs. The mod may be added to an existing save (can_add_to_saved_game=true), but it must not be removed from a save after Grid Battery entities have been saved (can_remove_from_saved_game=false). Keep a backup before enabling it in an important save.

Localization
The mod includes English, Russian, and 31 additional localizations. The selected translation is loaded from an editable flat [key, value] JSON catalog in the Localization directory during prototype registration; unsupported cultures fall back to the external English catalog. Change the game language, then restart or reload the mod/game so the prototypes are registered again.

Visuals
The release ships three distinct Grid Battery production models in its runtime asset bundle. T1 uses a rugged lead-acid (Pb) cell-bank model, T2 uses a serviceable Li-ion rack model, and T3 uses a sealed Solid-State cabinet model. All three models fit the 5x4 battery footprint and have no fluid ports. Runtime model assets are loaded from AssetBundles\mafi_bundles.manifest and the extensionless AssetBundles\gridbattery_models_* bundle.

Troubleshooting
Read the newest log in `%APPDATA%\Captain of Industry\Logs`. Report the first GridBattery exception together with the game version, mod version, selected language, and reproduction steps.

License
Source code is licensed under the MIT License. Original Grid Battery models, textures, icons, previews, and other visual assets are licensed under CC BY 4.0. Copyright and attribution: LordXaosa. See LICENSE and LICENSE-ASSETS included with the mod.
