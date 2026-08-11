# Configurable Machine Outputs

Configurable Machine Outputs adds per-machine routing controls for compatible output ports in Captain of Industry.

## Features

- Automatically detects compatible **Pipe**, **Loose**, and **Flat** output groups.
- Routes outputs independently for each placed machine.
- Displays the product currently assigned to each physical output port.
- Hides groups when swapping the currently assigned outputs would have no effect.
- Updates routing controls immediately when assigned recipes change.
- Preserves custom routing across saving, loading, and machine upgrades.
- Removes saved routing data after a configured machine is fully deconstructed.
- Prevents routing changes while the machine still contains products in its internal output buffers.

## Installation

1. Close Captain of Industry.
2. Extract the `ConfigurableMachineOutputs` folder into:

   `%APPDATA%\Captain of Industry\Mods`

3. Start the game and enable **Configurable Machine Outputs** in the Mods menu.

The resulting layout should be:

```text
Mods/
└── ConfigurableMachineOutputs/
    ├── ConfigurableMachineOutputs.dll
    ├── 0Harmony.dll
    ├── manifest.json
    ├── config.json
    ├── README.md
    └── CHANGELOG.md
```

## Usage

Select a supported machine. Routing buttons appear in the machine inspector for compatible output groups.

A button such as:

```text
Pipe X ← Low-pressure steam [Y]
```

means physical port **X** is currently receiving the recipe output originally assigned to port **Y**.

Click a routing button to cycle that physical port through compatible recipe outputs. The displaced assignment is exchanged automatically, so every source remains assigned to exactly one physical port.

Use **Reset output routing** to restore the machine's original port assignments.

## Safety

The machine's internal output buffers must be empty before routing can be changed or reset. Connected pipes or conveyors may still contain the previous product, so empty or disconnect downstream transport sections when cross-contamination matters.

## Notes

- `Unused` means the currently assigned recipe does not use that configurable physical port.
- Construction previews remain unchanged and show the machine's normal possible outputs.
- Routing is saved per machine rather than globally.
- The mod can be added to an existing save.
- Removing the mod from a save after use is not supported.

## Compatibility

Tested with Captain of Industry **0.8.5**.
