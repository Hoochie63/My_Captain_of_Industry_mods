# Cargo Helicopter 0.13.4

Cargo Helicopter adds three tiers of flying cargo vehicles to **Captain of Industry 0.8.5**.
They use the normal truck logistics system, but can cross cliffs, mountains, buildings and water.

## Features

- Three tiers with capacities of 40, 90 and 180 units.
- Route-aware flight altitude and smooth climb/descent.
- Separate container, fluid tank and log sling models.
- Winch animation: cargo is lowered before transfer and reeled up before departure.
- Multiple helicopters can load, unload and refuel at the same building simultaneously.
- Open Heliport T1 with two production lanes, twenty visible parking stands and five four-layer equipment columns.
- Open Heliport T2 with four production lanes, forty visible parking stands and ten concurrent equipment columns.
- Returning waves use per-column FIFO holding points: the next aircraft approaches as soon as the previous one clears
  the storage cell, without repeating the idle debounce or waiting for its full stand landing.
- Every helicopter is bound to its producing heliport before it becomes visible to the global dispatcher. It cannot
  switch between T1/T2 during ordinary work; destroying its home explicitly migrates it to compatible free capacity.
- If a home is demolished before a replacement has room, its aircraft climb to a safe equipment-free holding state
  and automatically continue to the next T1/T2 as soon as real stand and C-yard capacity becomes available.
- Containers and tanks are lowered onto the cargo deck before storage and retrieved before the next assignment.
- Landing/refuelling API for helipad and HQ integrations.
- Rotor animation, banking, wind, cargo swing and downwash.

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

- Minimum and verified game version: **0.8.5**.
- Can be added to an existing save.
- Do not remove it from a save that already contains its prototypes or vehicles.
- The aircraft is simulated as a truck in X/Z; the flight altitude and aircraft visuals are
  supplied by the mod.

## Development

Set the `COI_ROOT` environment variable to the Captain of Industry installation directory, then run:

```powershell
..\dotnet\dotnet.exe build .\CargoHelicopter.csproj -c Release
.\build-release.ps1
```

The build deploys a complete mod folder, including AssetBundles and all tier icons. The release
script creates a validated versioned ZIP without overwriting older releases.

See [CHANGELOG.md](CHANGELOG.md) for release notes.
