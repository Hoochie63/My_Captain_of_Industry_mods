# Cargo Helicopter 0.19.9

Cargo Helicopter adds three tiers of flying cargo vehicles to **Captain of Industry 0.8.5-0.8.6
(Update 4.2)**.
They use the normal truck logistics system, but can cross cliffs, mountains, buildings and water.

## Features

- Three tiers with capacities of 40, 90 and 180 units.
- Direct point-to-point air routes: the game still chooses the logistics destination, but both newly calculated
  and already-active/saved ground or road waypoints are discarded. Terrain and roofs affect altitude only, never
  the horizontal course.
- Route-aware flight altitude and smooth climb/descent.
- Separate container, fluid tank and log sling models.
- Winch animation: cargo is lowered before transfer and reeled up before departure.
- Multiple helicopters can load, unload and refuel at the same building simultaneously.
- Open Heliport T1 with two production lanes, twenty visible parking stands and five four-layer equipment columns.
- Open Heliport T2 with four production lanes, forty visible parking stands and ten concurrent equipment columns.
- Open Heliports use a full model-sized, terrain-anchored footprint: the complete foundation must share one ground
  plane before placement is accepted, so the platform cannot hang over a valley or cut through a hillside. They also
  cannot be quick-built through the Unity shortcut; Vehicle Parts/Rubber inputs, construction time and scrap-material
  outputs are mandatory.
- Returning waves use per-column FIFO holding points: the next aircraft approaches as soon as the previous one clears
  the storage cell, without repeating the idle debounce or waiting for its full stand landing.
- Every helicopter is bound to its producing heliport before it becomes visible to the global dispatcher. It cannot
  switch between T1/T2 during ordinary work; destroying its home explicitly migrates it to compatible free capacity.
- If a home is demolished before a replacement has room, its aircraft climb to a safe equipment-free holding state
  and automatically continue to the next T1/T2 as soon as real stand and C-yard capacity becomes available.
- Containers and tanks are lowered onto the cargo deck before storage and retrieved before the next assignment.
- Landing/refuelling API for helipad and HQ integrations.
- Scrap and replacement flights return to an Open Heliport, land on its deck and roll to the despawn point before
  the vehicle is removed and its recoverable materials are placed in the depot outputs.
- Rotor animation, banking, wind, gently damped cargo sling and downwash.

## Installation

1. Close the game.
2. Extract the `CargoHelicopter` folder into `%APPDATA%\Captain of Industry\Mods`.
3. Start the game and enable **Cargo Helicopter** for the save.

The final layout must contain `manifest.json`, both DLL files, the three `heli_icon*.png`
files and the complete `AssetBundles` directory in the same `CargoHelicopter` folder.

## Usage

Build either Open Heliport tier and queue helicopters in its production interface. Helicopters are not available in the
vanilla Vehicle Depot / Auto Vehicle Factory. They use the normal pickup, delivery, fuel and truck-group rules.

## Compatibility

- Supported game versions: **0.8.5-0.8.6c**; built and locally API-verified against the latest public
  **0.8.6c / Update 4.2** files. The direct-flight API is also present in the 0.8.6a/b hotfixes.
- Cheat++ is optional. When both mods are enabled, Cargo Helicopter declares the correct load order,
  ships the same Harmony 2.4.2 runtime and retries Cheat++'s C/Overlord toolbar insertion if the
  Update 4.2 HUD was not ready for its first attempt.
- Can be added to an existing save.
- Do not remove it from a save that already contains its prototypes or vehicles.
- The normal truck scheduler and driving integrator are retained for logistics/save compatibility, but the active
  helicopter target is forced to the final logistics goal and any live road-driving state is cleared. Flight
  altitude and visuals are supplied by the mod and clear the terrain independently.

## Development

Set the `COI_ROOT` environment variable to the Captain of Industry installation directory, then run:

```powershell
..\dotnet\dotnet.exe build .\CargoHelicopter.csproj -c Release
.\build-release.ps1
```

The build deploys a complete mod folder, including AssetBundles and all tier icons. The release
script creates a validated versioned ZIP without overwriting older releases.

See [CHANGELOG.md](CHANGELOG.md) for release notes.
