# Cargo Helicopter 0.20.0

Cargo Helicopter adds five diesel and hydrogen flying cargo variants across three capacity tiers to
**Captain of Industry 0.8.7 (Update 4.2)**.
They use the normal truck logistics system, but can cross cliffs, mountains, buildings and water.

[Download on CoI Hub](https://coigame.com/Mod/1102/Cargo-Helicopter) ·
[Source code](https://github.com/Pique1804/Cargo-Helicopter) ·
[Changelog](CHANGELOG.md)

## Features

- Three capacity tiers (40, 90 and 180 units) with diesel I/II/III and hydrogen II H/III H variants.
- Hydrogen helicopters use the game's hydrogen fuel stations and research, consume 125% hydrogen-equivalent tank
  capacity and produce no Polluted Air exhaust.
- Direct point-to-point air routes: the game still chooses the logistics destination, but both newly calculated
  and already-active/saved ground or road waypoints are discarded. Terrain and roofs affect altitude only, never
  the horizontal course.
- Route-aware flight altitude and smooth climb/descent.
- Separate container, fluid tank and log sling models.
- Winch animation: cargo is lowered before transfer and reeled up before departure.
- Multiple helicopters can load, unload and refuel at the same building simultaneously.
- Open Heliport T1 with two production lanes, twenty visible parking stands and five four-layer equipment columns.
- Open Heliport T2 with four production lanes, forty visible parking stands and ten concurrent equipment columns.
- The heliport inspector shows permanent home-fleet occupancy (`19/20`) and an exact localized reason when production
  is waiting for stand markers, world/local fleet capacity or an assembly lane.
- An amber inward arrow marks only a parking stand already reserved by an incoming helicopter; empty and parked
  stands remain visually self-explanatory without redundant colour plates or an AssetBundle rebuild.
- Built-in English and Russian localization for vehicles, heliports and the new capacity/status UI.
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
2. Download the current release from [CoI Hub](https://coigame.com/Mod/1102/Cargo-Helicopter).
3. Extract the `CargoHelicopter` folder into `%APPDATA%\Captain of Industry\Mods`.
4. Start the game and enable **Cargo Helicopter** for the save.

The final layout must contain `manifest.json`, both DLL files, the three `heli_icon*.png` files, the complete
`AssetBundles` directory and `translations/en.json` plus `translations/ru.json` in the same `CargoHelicopter` folder.

## Usage

Build either Open Heliport tier and queue helicopters in its production interface. Helicopters are not available in the
vanilla Vehicle Depot / Auto Vehicle Factory. They use the normal pickup, delivery, fuel and truck-group rules.

Detailed per-aircraft flight and cargo-yard logs are disabled by default. When diagnosing an animation or dispatch
problem, enable `verboseFlightLogs` in the mod settings, reload the save, reproduce the issue once and attach the game
log. Disable it again for normal play; warnings and periodic heliport heartbeats are always retained.

## Compatibility

- Supported game version: **0.8.7**; built and locally API-verified against public Steam build
  **24719404 / v0.8.7a**. CoI Hub compatibility uses `0.8.7`, which covers lettered 0.8.7 hotfixes.
- Cheat++ is optional. When both mods are enabled, Cargo Helicopter declares the correct load order,
  ships the same Harmony 2.4.2 runtime and retries Cheat++'s C/Overlord toolbar insertion if the
  Update 4.2 HUD was not ready for its first attempt.
- Can be added to an existing save.
- Do not remove it from a save that already contains its prototypes or vehicles.
- The normal truck scheduler and driving integrator are retained for logistics/save compatibility, but the active
  helicopter route is kept straight through bounded collinear SIM legs and any live road-driving state is cleared. Flight
  altitude and visuals are supplied by the mod and clear the terrain independently.

## Development

Requirements:

- Windows with .NET SDK 8 and the .NET Framework 4.8 targeting pack.
- A legal Captain of Industry installation.
- `COI_ROOT` set to the game's installation directory.
- Unity `6000.0.66f1` only when rebuilding the AssetBundles in `HeliportUnity/`.

Build the C# project from the repository root:

```powershell
dotnet build .\CargoHelicopter.csproj -c Release
.\build-release.ps1
```

If `dotnet` on PATH has no SDK, set `DOTNET_EXE` to a .NET SDK executable. The release script also
recognizes a private SDK at `.dotnet\dotnet.exe` or `..\dotnet\dotnet.exe`; those SDK directories are
local development tools and must not be committed.

The regular build deploys a complete mod folder to `%APPDATA%\Captain of Industry\Mods` unless
`-p:DeployMod=false` is passed. The release script validates the DLL version, Harmony version,
AssetBundle Unity headers and ZIP contents, then writes `dist\CargoHelicopter_<version>.zip` without
overwriting an existing release.

### Maintainer automation

The `CoI Hub forum sync` GitHub Action checks the public Cargo Helicopter board every three hours.
New, unquoted posts after CoI Hub post `4516` become GitHub issues; quoted replies become comments
when their referenced post has already been imported. Imported content is labelled
`source:coi-hub` and `needs-triage`. Hidden topic/post markers prevent duplicates. The importer uses
public `/Forum/Mods/CargoHelicopter` and `/Topic/*` pages only; it has no CoI Hub credentials. Each run
imports at most 25 oldest pending posts, so a backlog drains safely over successive runs.

Run its parser tests locally without network access:

```powershell
python -m unittest discover -s tests -p "test_coi_hub_sync.py" -v
```

For a release, merge to a clean `main` branch, update `manifest.json` and the three version fields in
`CargoHelicopter.csproj`, authenticate GitHub CLI, then run on the trusted Windows development PC:

```powershell
.\tools\new-draft-release.ps1
```

The helper verifies the branch, worktree, version/tag and repository, calls `build-release.ps1`,
writes a SHA-256 checksum and creates a **draft** GitHub Release. It never publishes the release.
CoI Hub upload and publication remain manual because the Hub does not provide a supported upload API.

Repository layout:

- Root: C# source, manifest, configuration, release script and runtime AssetBundles.
- `HeliportUnity/`: Unity source assets, package manifest and project settings.
- `build/`: Blender helper scripts used to inspect and prepare the helicopter model.
- `licenses/`: licenses for redistributed third-party runtime dependencies.

## Security and transparency

Cargo Helicopter uses Harmony patches and reflection because the current modding API does not expose
all vehicle-flight, dispatch and heliport hooks required by the mod. It reads its bundled icon files
from the mod directory. The mod does not make external network connections.

No Captain of Industry DLLs or original unmodified game assets are included in this repository. Build
references are resolved from the user's own installation through `COI_ROOT`.

## License and legal notice

The original Cargo Helicopter code and assets are published under the
[Captain of Industry Open License (COI-Open)](LICENSE). Third-party components retain their own
licenses and attributions; see [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).

This Mod includes short excerpts or references to Captain of Industry Game Code. Any such Game Code
is © MaFi Games and is used only under the
[Captain of Industry Modding Policy](https://coigame.com/Legal/Modding-Policy).

See [CHANGELOG.md](CHANGELOG.md) for release notes.
