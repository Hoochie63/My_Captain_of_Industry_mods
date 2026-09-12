# Changelog

## 1.0.0

- Declares Factory Lens as a true `IsUiOnly` mod instead of a data/content mod, so it cannot participate in simulation state or headless save verification.
- Defers all UI and diagnostics startup until the game has completely finished deserialization and `InitAfterLoad` finalization.
- Suspends diagnostics while the game captures its synchronous save snapshot, with completion and deterministic-snapshot fallback guards.
- Ignores destroyed, phantom, or invalid machine, recipe, product, and port prototypes instead of inspecting partially available mod content.
- Fixed flow consumers such as smoke stacks showing a misleading stalled-pipe diagnosis while normally waiting for exhaust.
- Added **Cause Trace**: selecting a problem isolates the affected building and the first diagnosed cause, draws the product path between them, and exposes one concrete corrective action.
- The selected row's arrow jumps to the first cause; clicking the selected row again exits focused trace mode.
- Added **Logistics Lens** using live game buffer and pathfinding signals: disabled import/export, assigned or reserved deliveries, no free trucks, no generated job, routes unreachable for every truck, stock reserved by another delivery, full destinations, and disconnected or stalled belts and pipes.
- Assigned deliveries and pickups remain normal waiting during their grace window instead of becoming false blockers.
- Added predictive warnings for filling outputs and depleting inputs. Forecasts require a sustained trend, are suppressed when transport is already scheduled, and disappear when the trend reverses.
- Added remaining recipe-cycle estimates and a rolling recurrence counter for the same problem.
- Predictive map labels pulse gently without notification spam.
- Expanded English and Russian localization, diagnostic logs, configuration, and automated coverage.

## 0.8.0

- Prepared the first public beta listing with final English and Russian COI Hub descriptions and original square cover art.
- Declares verified compatibility with the current public Steam release, Captain of Industry 0.8.7a build 24719404, while retaining minimum support for 0.8.5.
- Expanded manifest metadata for the in-game COI Hub mod manager. No simulation or save-format behavior changed.

## 0.7.2

- Keeps the complete Factory Lens frame inside the visible UI viewport after every width change.
- Revalidates saved window positions when the panel opens, preventing an edge-positioned panel from reopening partly off-screen.

## 0.7.1

- Added safe right padding between diagnostic cards, navigation buttons, and the vertical scrollbar.
- Fixed slider readouts to show the actual width and opacity instead of normalized track percentages.
- Replaced zero-basis summary columns with fixed compact columns to prevent vertical text collapse at 440 pixels.

## 0.7.0

- Changed the default and minimum panel width to 440 pixels.
- Replaced the text overlay controls with compact native map-pin and content-mode icon buttons with explanatory tooltips.
- Replaced width and opacity minus/plus buttons with persistent native sliders.
- Stabilized the 440-pixel layout with fixed slider widths, a two-by-two summary, and non-shrinking duration and navigation controls.
- Replaced inconsistent **Open** text buttons with equal square native **Go to** arrow icons.
- Made the diagnostic area use an always-visible vertical scrollbar and reserve its width so content does not jump.
- Stops rebuilding the control bar for every factory snapshot, keeping sliders usable while the diagnostics continue updating.

## 0.6.1

- Removed the large clickable **Can ignore** and **Delivery** rows because they duplicated the top counters and actionable-building list.
- Added persistent horizontal width controls from 420 to 1000 pixels in 40-pixel steps.
- Changed the default window width from 800 to 680 pixels and lets controls, counters, and cause chips wrap at narrow widths.
- Fixed selected recipe-chain highlighting so it cannot recolor two identical problems blue and red; actionable icons now always keep their state color.

## 0.6.0

- Rebuilt the window as an 800×620 at-a-glance HUD instead of a report.
- Removed visible explanations, instructions, movement hints, search, and other paragraph text.
- Reduced each problem to a compact building header plus one status, duration, and **Open** action.
- Moved the full explanation and recommended action into the building-row hover tooltip.
- Replaced the two large graph panels with one 42-pixel trend and short cause counters.
- Made cause counters mutually exclusive so their total matches the number of actionable buildings.
- Hides normal-waiting and delivery sections entirely when they are empty.
- Shortened map, overlay-mode, opacity, summary, waiting, and delivery labels.

## 0.5.0

- Removed the remaining navigation tabs and placed summary, graphs, problems, normal waits, and deliveries on one top-to-bottom page.
- Added separate persistent Alt-overlay choices: icons plus labels, icons only, or labels only.
- Added persistent in-game window opacity controls from 55% to 100%.
- Made the existing movable, position-saving, and pinnable window behavior explicit in the UI.
- Restored a plain-language problem-history graph where lower bars are better.
- Restored a cause graph with named categories and exact building counts instead of diagnostic jargon.
- Added a compact normal-waiting section so flow consumers such as exhaust stacks are visibly separated from real problems.
- Moved all window, graph, action, and map-label strings into keyed English and Russian UTF-8 localization catalogs.
- Simplified the map overlay to its core attention view and removed obsolete utilization/logistics view modes.

## 0.4.0

- Replaced the analytics-heavy default dashboard with a plain task list: what stopped, why, and what to do.
- Reduced the main navigation to **Needs action**, **All buildings**, **Deliveries**, and an explicit map-label toggle.
- Removed low/high-load and priority sorting controls from the player-facing window.
- Replaced reliability percentages, history charts, and impact graphs with one short summary written in player language.
- Added concise status, explanation, recommended action, state duration, and a visible **Show and open** action to every card.
- Renamed the misleading truck “queue” metric to **Idle trucks** and explains that these vehicles are waiting for work.
- Added crisp, collision-reduced map labels such as **Needs unloading: Iron** and **Waiting for: Air pollution** next to product icons.
- Fixed exhaust stacks and other flow consumers with no real output buffer so persistent input waiting remains normal instead of becoming a warning.
- Reduced the window to 940×740 now that advanced charts no longer occupy the default view.

## 0.3.1

- Replaced raw `Machine.State != Working` problem counting with dependency-aware impact levels.
- Added explicit Critical, Blocking, Attention, Waiting, and Buffering classifications.
- Treats input-only flow consumers such as exhaust/sink pipes as normal waiting, not factory faults.
- Uses the active recipe graph and truck reservations to explain whether a full output is blocked downstream, waiting for export, or has no active recipe consumer.
- Treats a persistent `OutputFull` state as a production blocker, including when no active recipe consumer is found.
- Added a grace period for transient input/output waits during normal production cycles.
- Reworked reliability so normal waiting and brief buffer transitions do not reduce the score.
- Added detailed impact explanations, state duration, and an explicit importance-level chart.
- Increased the operations-center window to 1080×840.
- Added concise impact-transition diagnostics to the game log for screenshot/log comparison.

## 0.3.0

- Rebuilt the window as a full factory operations center using native CoI UI components.
- Added factory-health, working-machine, issue, and truck-queue KPI cards.
- Added a real color-coded health-history graph and issue-distribution bars.
- Added machine, recipe, product, state, and diagnosis search.
- Added priority, name, low-load, and high-load sorting.
- Replaced text buffer bars with graphical utilization and buffer meters.
- Added a step-by-step root-cause panel with a recommended player action.
- Expanded diagnosis to cover delivery failures, capacity shortages, external supply, upstream failures, recipe cycles, and long chains.
- Added interactive map-chain highlighting, directional world-space arrows, and root-machine focus.
- Added contextual map legends and proportional truck-activity bars.
- Added a ranked live import/export request queue with unserved-request detection.

## 0.2.0

- Replaced the sparse machine list with compact cards and product icons.
- Added utilization and input/output buffer bars to every machine card.
- Added upstream recipe-chain analysis for likely input-shortage root causes.
- Distinguished producer shortages, insufficient production capacity, and downstream transport or priority issues.
- Reduced the diagnostics window size and limited filters to modes where they are relevant.

## 0.1.0

- Added four map overlay modes: problems, recipes, utilization, and logistics.
- Added live machine-state diagnostics and bottleneck filters.
- Added input/output buffer pressure analysis and monthly truck statistics.
- Added configurable hotkeys, refresh rate, thresholds, icon cap, and language.
- Added English and Russian interface text.
