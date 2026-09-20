OMNI-TOOL
=========

Omni-Tool combines repetitive inspector actions into one native world tool:

- Unity Quick Repair
- Move Vehicle
- Assign Logistics Zone
- Vehicle Depot Orders (scrap, cancel, or replace)
- Unity Recovery
- Unity Cargo Unload
- Downgrade Buildings
- Building Settings Brush (pause/unpause, priority, truck policies, and storage alerts)
- Cancel Deconstruction

When Cheat++ is loaded—or standalone access is enabled through Mod Config—the Settings tab can explicitly reveal seven sandbox and creative modes:

- Mine / Dump Designations
- Remove Marked Trees
- Nature Sweeper
- Storage Magic Wand
- Tree Painter
- Terrain Sculpting
- Terrain Eraser (experimental)

When Cheat++ itself is loaded, a conditional Cheat++ Keep Full / Empty card can mass-apply Keep Full, Keep Empty, or Normal states.

Terrain Sculpting can save up to twenty material favorites shared by Raise, Lower, Flatten, and Paint Material. Use the gold star to add the current material; left-click a favorite to select it and right-click it to remove it.

Tree Painter offers Single Tree for its original exact-spacing workflow and Mixed Forest for selecting several species at once. Mixed Forest uses randomized density to avoid plantation-like rows and can save up to seven tree-and-density bundles. Left-click a colored bundle to load it and right-click to remove it.

HOW TO USE
----------

1. Left-click the Omni-Tool toolbar button.
2. Choose a mode from the independent Omni-Belt. The last mode is remembered.
3. Left-click one compatible entity, or drag a box around many compatible entities.
4. In Move Vehicle mode, click the destination after selecting the vehicles.
5. In Assign Logistics Zone mode, choose Default or any current custom zone, then click or box-select trucks. Default clears their custom zone assignment.
6. In Vehicle Depot Orders mode, choose Mark for scrapping, Cancel scrapping, Replace vehicle, or Cancel replacing before selecting. Mark and Replace each have a separate optional exact vehicle-type filter: enable it, press the pick button, and click a vehicle in the world before making a large selection. Replace never targets a vehicle whose prototype is identical to the chosen replacement; different variants remain valid. Normal compatibility and depot requirements still apply.
   Building Settings Brush has independent apply checkboxes for Pause / Unpause, general priority, truck import/export policies, and empty/full storage alerts. It can optionally filter by an exact building type picked from the world and save up to seven complete settings bundles. Only checked settings change, and unsupported targets are skipped.
7. In Mine / Dump Designations mode, choose a loose product in the Micro HUD, then repeatedly drag over dumping or mining designations to complete them.
8. Remove Marked Trees shows the live number of trees already designated for harvesting and removes them with one explicit button.
9. Nature Sweeper removes trees, stumps, rocks, bushes, and decorative terrain props inside the dragged area through a small adaptive per-frame time budget. Escape cancels the remaining queue.
10. Storage Magic Wand fills compatible box-selected storages with the chosen product. Its explicitly selected EMPTY mode clears their cargo.
11. Tree Painter plants the chosen tree across the dragged area using a remembered 1–30 tile spacing slider. Choose Box Selection or Round Brush in the Micro HUD.
12. Restore Terrain remains implemented for future investigation but is dormant and cannot be equipped or cycled in the 0.7 release line.
13. Terrain Sculpting raises, lowers, flattens, or paints the dragged terrain using the compact operation, material, height-step, and Box/Round Brush controls. Its selected terrain material applies during every operation, so Raise can build mountains from gravel, ore, or any other listed material.
14. Terrain Eraser removes selected terrain above a remembered absolute target. Its precision range is -10 to 0; enable Expanded height range for -50 to +50. It does not raise terrain already below the target.
15. Tree Painter, Terrain Sculpting, and Terrain Eraser share a configurable 1–10 step Undo history (default 3). Pause before terrain editing for reliable undo: simulation settling and buildings already destroyed by collapse are permanent.
16. Mine / Dump Designations can keep fourteen favorite terrain products, while Storage Magic Wand can keep twenty-eight favorite storage products in four rows of seven. Use the gold star to add the current product; left-click a favorite to select it and right-click it to remove it.
17. When Cheat++ is loaded, Cheat++ Keep Full / Empty mass-applies Keep Full, Keep Empty, or Normal to selected storages and updates Cheat++'s normal labels.
18. Hold Ctrl and left-drag a contextual palette header to move it. Its position is remembered.
19. Right-click or press Escape to cancel or leave the tool.

Right-click the toolbar button to open three focused tabs: Tools, Omni-Belt Settings, and Settings. Tools contains full-width cards, enable checkboxes, hover explanations, icon colors, Hide Unequipped, and drag ordering. Omni-Belt Settings owns the active profile's category visibility and selective actions, while Settings owns global icon, Selection Details, the conditional sandbox master and Undo-history preferences, plus factory-profile recovery. Right-click remains a guaranteed recovery path even if the Settings Shortcut card is disabled.

The true Omni-Belt is a separate native-looking panel, so moving it never moves building, transport, copy/paste, or other contextual placement shortcuts. Hold Ctrl and left-drag anywhere on the belt to reposition it; its location is remembered and kept on-screen. The Profile Switcher and Settings Shortcut are yellow in the installed factory profiles and can each be hidden, recolored, and reordered like ordinary tool cards. The Profile Switcher remains available whenever more than one profile exists. Tool cards use blue Vehicle, cyan Logistics, purple Buildings, and amber Sandbox markers so categories remain obvious after custom reordering, and every tool has its own profile-saved icon color picker.

The compact contextual selection palette is called the Micro HUD. During normal tool use, presses beginning inside it or its dropdowns cannot accidentally paint, fill, sweep, or select terrain underneath. Opening the full Omni-Tool settings window cancels and locks every Omni-Tool world action across the entire screen while leaving a dimmed Omni-Belt preview visible for live editing. Mode buttons cannot activate until Settings closes, and closing Settings never silently resumes the selected tool. Any area drag already in progress is cancelled safely when Settings takes control.

The permanent Omni-Belt Profiles tray sits below every tab. A fresh install starts with curated Vehicle Tools and Building Tools profiles. If Cheat++ or Mod Config grants sandbox eligibility, it also receives a prepared Terraforming profile that starts outside profile cycling and stays dormant while the global sandbox master is off. Every change to the active profile—including visible categories, tool order, enabled modes, icon colors, favorites, contextual choices, and both panel positions—is saved automatically after a short debounce and is flushed before switching. Uncheck a profile's unlabeled cycle checkbox to keep it loadable from the tray while skipping it with the profile shortcut. Omni-Belt Settings can reset the active profile's order or colors, or enable every currently authorized tool. Global Settings can rebuild only the profile catalog after two explicit confirmations; ordinary global settings are preserved.

Hide Unequipped removes disabled cards from the editor without changing their settings or hidden relative order. Reorder remains pinned above the scrolling cards, applies to tools, Omni-Belt utility cards, and open profile-tray rows, and only starts from a visible drag handle. The bottom-right grip resizes the wider settings window horizontally and vertically.

The Vehicle Tools profile contains Move Vehicles, Unity Quick Repair, Unity Recovery, Unity Cargo Unload, Scrap / Replace, and Assign Logistics Zone. The Building Tools profile contains Unity Quick Repair, Building Settings Brush, Downgrade Buildings, and Cancel Deconstruction, plus eligible storage cheat tools. The Terraforming profile contains Mine / Dump Designations, Terrain Sculpting, Terrain Eraser, Tree Painter, and Nature Sweeper; Remove Marked Trees is prepared but starts disabled, as do both storage tools in that profile. Vehicle/Logistics, Buildings, and Sandbox are separately visible per profile. Unity Quick Repair permanently carries Vehicle and Buildings tags; both storage tools permanently carry Sandbox and Buildings tags. A dual-tag card stays visible if either category is shown.

Sandbox eligibility requires either Cheat++ detection or the disabled-by-default Sandbox cheat tools Mod Config option. Only eligible installs show the global Enable Sandbox Tools master and Creative Undo setting. The global master starts off and gates sandbox-class actions across every profile; it does not erase the dormant Terraforming setup. Cheat++ Keep Full / Empty still requires Cheat++ itself.

When Keybind Framework is loaded, assign both shortcuts under Settings -> Mod keybinds -> Omni-Tool. Without the framework, the same two entries appear under native Settings -> Controls -> Omni-Tool instead. The two locations are mutually exclusive. Both shortcuts are deliberately unassigned by default. "Activate / Cycle Omni-Tool" activates the remembered enabled mode, then advances to the next enabled mode on every additional press. "Cycle Omni-Belt" advances through profiles whose unlabeled cycle checkbox is enabled; it does nothing when fewer than two profiles participate. Escape always closes the active world tool.

Compatible targets highlight before selection. The global Selection Details option shows target counts for applicable tools, Unity totals where relevant, and how many Scrap or Logistics targets will actually change. Turning it off skips that extra aggregation and formatting.
When no compatible target is highlighted, a small mode-matching icon beside the mouse pointer confirms that Omni-Tool is active.

The toolbar icon can follow the selected mode. Its framed hover tooltip explains the left- and right-click actions, then shows the active Activate / Cycle binding or points to the correct setup page when it is unassigned. Right-click the Omni-Tool toolbar button to open its settings.

LANGUAGES
---------

Omni-Tool automatically follows the game's selected language. Every settings label, mode name, tooltip, contextual palette, selection counter, Unity-cost line, profile control, and keybind description is translated.

Supported languages: Catalan, Czech, German, English, Spanish, Estonian, French, Hungarian, Italian, Japanese, Korean, Norwegian Bokmål, Dutch, Polish, Brazilian Portuguese, Russian, Swedish, Turkish, Ukrainian, Simplified Chinese, and Traditional Chinese.

Each JSON catalog uses explicit "English string": "translated string" entries so bilingual contributors can immediately see what every value means and update individual translations without relying on array position.

Game-native product, tree, terrain-material, and logistics-zone names keep using the game's own localized text. If a catalog cannot be loaded, Omni-Tool safely falls back to English without preventing the mod from loading.

MOD INTEGRATION
---------------

Omni-Tool has no required mod dependencies. Keybind Framework 2.0.2 or newer is optional: when detected at load, Omni-Tool registers exclusively on its Mod keybinds page and suppresses its native Controls category. Without the framework, Omni-Tool creates only the native Controls category. It never activates both backends during the same load. KeyBound is also optional; its normal toolbar discovery can expose the Omni-Tool toolbar button as a separate conventional toggle binding and Live Rebind target.

Cheat++ is optional. A successfully loaded Cheat++ installation makes the global sandbox master available under Settings and enables the Cheat++ storage card. Without Cheat++, set Sandbox cheat tools to true in Mod Config and reload the game to grant general sandbox eligibility. If neither condition is true, Enable Sandbox Tools, Creative undo history, the Sandbox category switch, and Terraforming factory profile are not shown. Per-profile category switches only organize visibility; they never bypass the global master or external eligibility. The extra Cheat++ Keep Full / Empty card calls the game's real storage cheat-mode method so Cheat++'s existing KF/KE label patch remains the source of its labels. Omni-Tool never opens Overlord.
