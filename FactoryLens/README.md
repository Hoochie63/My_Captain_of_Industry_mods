# Factory Lens

Factory Lens traces a stopped production chain to its first cause, explains the exact logistics failure, and warns when a buffer is heading toward a stop. It does not change the simulation or save format.

## Controls

- `F8`: open or close Factory Lens.
- `Left Alt`: show or hide the selected map overlay.
- Both keys can be changed in the mod configuration.

The two compact icon buttons control the map overlay. The map-pin button shows or hides downtime
reasons above buildings. The neighboring button switches between **icons + labels**, **icons only**,
and **labels only**. Hovering either button explains its current state. Both choices are saved locally.

Native sliders change opacity from 55% to 100% in 5% steps and width from 440 to 1000 pixels in
20-pixel steps. Their readouts show the actual percentage and pixel width. Their fixed layout
remains stable while the panel itself changes width. If an edge-positioned panel grows, it is
automatically shifted back inside the visible screen instead of expanding off-screen.

Drag the title bar to move the window. The native pin in the top-right keeps it open while playing.
Window position, width, overlay contents, and opacity are kept between sessions, so the panel
can be used as a semi-transparent bottom-left monitor.

## Main window

The default 440×620 window is an at-a-glance HUD, not a report. It has no tabs, search modes, sorting,
instructions, or visible explanation paragraphs. It shows:

1. four current counts in a stable two-by-two summary;
2. a 42-pixel problem trend and short cause counters;
3. one compact row per actionable building.

Normal waiting and delivery details are intentionally not repeated as large clickable rows. Their
counts remain in the summary, while the list is reserved for buildings that require intervention.

Important buildings are always shown first. Each row contains only the building name, exact status,
product, state duration, recurrence badge when relevant, and an equal square arrow button. Hovering
reveals supporting details. Clicking a row selects its Cause Trace; the selected row shows exactly one
corrective action, and its arrow jumps to the first cause. Clicking the selected row again exits the
focused trace. The diagnostic area has a persistent vertical scrollbar and supports mouse-wheel
scrolling without shifting card widths.

Cause counters assign one primary cause to every actionable building, so the categories no longer
double-count a machine that happens to have pressure on more than one buffer.

## Map labels

The map overlay combines the native product icon with a short text label, for example:

- **Needs unloading: Iron**;
- **Needs material: Coal**;
- **Waiting for: Air pollution**;
- **No electricity**;
- **Needs repair**.

Labels stay screen-readable at any zoom and are repositioned to reduce overlap. Red and orange labels require action. Blue labels describe normal waiting and can be ignored.
Selecting a production chain can add icons for otherwise hidden producers, but it never overrides
the state color of an actionable building.

While a problem is selected, unrelated overlay markers are hidden. The map keeps only the problem,
the first diagnosed cause, and the directional product path between them. Predictive labels use a
gentle pulse instead of notification spam.

## Localization

Window, graph, action, and map-label text uses keyed UTF-8 catalogs in `Localization/en.tsv` and
`Localization/ru.tsv`. `language` can be `auto`, `en`, `ru`, or the code of another catalog added
to the project. Catalog keys are checked by tests so English and Russian cannot silently drift.

## How waiting is classified

- **Critical**: maintenance failure, invalid placement, or a shortage of power, computing, or workers.
- **Blocking**: a full output or confirmed interruption has stopped production.
- **Attention**: a productive building is persistently missing material or is close to blocking.
- **Waiting**: an input-only flow consumer, paused building, or short delivery gap. This is informational.
- **Buffering**: an output has only just filled and is still inside the grace period.

Exhaust stacks, flares, and similar consumers can expose a formal recipe output without having a real output buffer. Factory Lens checks for an actual buffered output, so these buildings remain **Waiting** instead of becoming false production warnings.

Soft input/output states must persist for the configured grace interval before becoming actionable. The default is five seconds.

## Cause Trace and Logistics Lens

For a real shortage, Cause Trace follows active recipes upstream and isolates the first useful cause.
For a blocked output it follows the affected product toward the destination. Logistics Lens reads the
same live buffer, reservation, pathfinding, and port state that the game uses, distinguishing:

- import or export disabled;
- a delivery or pickup already assigned;
- no free trucks versus free trucks with no generated job;
- a route marked unreachable for every truck;
- stock already reserved by another delivery;
- a full destination buffer;
- a disconnected or stalled belt or pipe;
- missing destination, external supply, or upstream capacity.

## Predictive warnings

Factory Lens observes input and output buffers over time. After a stable trend is established it can
show when an output is expected to fill, how many recipe cycles remain in a low input reserve, and how
often the same problem has returned inside the configured rolling window. A new delivery, pickup, or
reversed trend automatically clears the prediction.

Factory Lens is UI-only. It can be added to or removed from an existing save.
Runtime setup starts only after save deserialization has fully completed. Diagnostics are suspended
while a save snapshot is captured, and phantom or invalid prototypes from unavailable content mods
are ignored rather than inspected.

## Compatibility

- Supports Captain of Industry 0.8.5 through 0.8.7a.
- Verified against the current public Steam release 0.8.7a, build 24719404.
- Requires no other mods and can be enabled or disabled on an existing save.

## Building

Set `COI_ROOT` to the Captain of Industry installation directory, then run:

```powershell
dotnet build FactoryLens.csproj -c Release -p:DeployMod=false
```

The assembly is written to `bin/Release/net48/FactoryLens.dll`. Use `-p:DeployMod=true` only for an intended in-game test installation.
