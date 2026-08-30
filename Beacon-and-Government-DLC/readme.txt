BEACON & GOVERNMENT DLC
Version 1.0.3

Expands Captain of Industry's Beacon into a placeable support network with
research-gated randomized supplies, Captain's Currency, rotating quick trades,
Government reward policies, regional infrastructure subsidies, real water-routed
cargo deliveries, and reusable government landmarks.

BEACON NETWORK

- Adds unlimited Relay Beacons using the vanilla Beacon model and icon.
- Relay Beacons are decorative while disarmed. Each armed Relay contributes
  0.25 Unity per month while the primary Beacon is operational.
- Government Office I and II can contribute 1.00 and 1.50 Unity per month.
- Unity is charged through the primary Beacon's native monthly consumer and
  is prorated when support changes during a Beacon cycle.
- The saved full-cycle average prevents last-second toggles from granting a
  full reward multiplier.
- The vanilla Beacon cycle duration and base refugee arrival remain unchanged.
- Network inspectors clearly separate current Unity cost, the saved full-cycle
  reward average, and Captain's Currency balance.
- Camera controls locate the primary Beacon and cycle through constructed
  Relays, Government Offices, Exchanges, or all supported sites.
- Network-wide controls can arm or disarm every supported building.
- Relay, Exchange, and Government inspectors use compact network shortcuts and
  an always-visible Currency, Beacon Network, and Cycle Reward summary.

RANDOMIZED BEACON REWARDS

- Every completed Beacon cycle grants one weighted useful product reward.
- A rare two-stage roll can grant additional distinct products, up to five
  total rewards at the highest eligible Unity scale.
- Rewards are restricted to products already unlocked and available in the
  current game.
- The curated pool includes ores, processed metals, fuels, food, scraps,
  household and medical supplies, Construction Parts I-IV, Electronics I-IV,
  Mechanical Parts, Vehicle Parts, Lab Equipment, Solar Cells, and other
  useful products.
- Lab Equipment and Vehicle Parts use only the highest currently unlocked
  tier. Other still-useful lower-tier products can remain eligible.
- Product quantities scale from the Beacon network's saved full-cycle average
  Unity rate and retain randomized variance.
- Product rewards and Captain's Currency appear in the native refugee-arrival
  reward window and are delivered through the constructed Shipyard.
- Visible lucky multi-product drops receive Busy Harbor, Thriving Harbor, or
  Grand Flotilla headings based on the number of material stacks. Captain's
  Currency does not count as a cargo stack.

CAPTAIN'S CURRENCY AND QUICK TRADES

- Completed Beacon cycles can award a small, variable amount of Captain's
  Currency from the same saved full-cycle average.
- Currency, armed support sites, cycle progress, and active Exchange offers are
  stored inside each individual save file.
- Every Captain's Exchange presents three research-gated quick-trade offers.
- Newly generated offers use dedicated per-product quantity and price ranges
  balanced independently from Beacon-cycle supply drops.
- Most quantities remain roughly 20% below their original values at their old
  Currency prices. Construction Parts I-IV receive another roughly 15% quantity
  reduction and higher prices to protect the intended early progression.
- Offers already stored in a save keep their current quantity and price until
  the next normal yearly refresh. Paid delivery orders are unchanged.
- Quick trades refresh once per in-game year and spend Captain's Currency
  immediately. Each purchase becomes a saved three-month delivery order.
- Each prepared order dispatches one transient vanilla cargo ship along a real
  water route from a safe ocean edge to the purchasing Captain's Exchange.
  Its berth follows the Exchange's orientation, it steers around terrain,
  pauses with the simulation, and scales with game speed.
- Cargo transfers to the constructed Shipyard at berth before the ship returns
  to open water. Routed ships are now a permanent part of quick trading; the
  temporary Show delivery ships checkbox has been retired.
- Paid orders persist across save/load. A removed target Exchange falls back
  to another constructed Exchange. A missing Shipyard, valid ocean berth, or
  reachable water route leaves the paid order safely pending.
- Before a save or autosave, B&G removes every transient courier and flushes
  its deferred pathfinding work. An unfinished inbound order remains durable
  and dispatches again, while delivered cargo cannot be duplicated.
- Purchased offers cannot be bought a second time before the next refresh.

GOVERNMENT REWARD POLICIES

- Government Office I and II provide three policies that redirect the next
  completed Beacon-cycle reward.
- Outbound Resettlement removes refugee arrivals while preserving product
  rewards and adds an independent 80% chance for one additional Coin.
- Immigration Incentives converts the cycle's rolled Currency into two
  refugees per Coin while preserving product rewards.
- Humanitarian Priority redirects the product shipment into additional
  refugees while preserving Captain's Currency.
- Outbound Resettlement is exclusive with both inward policies. Immigration
  Incentives and Humanitarian Priority may operate together.
- The Government Office inspector and Ctrl+F5 global window use the same
  native-style Regional Policies cards and synchronized 0/I controls.

REGIONAL ISLAND SUBSIDIES

- Government Offices can spend Captain's Currency on imported Power Generation
  and Computing Capacity.
- One term lasts 24 calendar months. Purchases can cover one to five terms and
  begin at the next calendar month even if the Beacon is paused or disabled.
- Native-style I-V selectors choose one to five terms. The selected total
  Captain's Currency cost appears in the card title before Purchase is applied.
- Queued contracts, active capacity, and expiry dates persist in the exact save
  snapshot. Pausing the Beacon does not extend an active fixed term.
- Regional prices and available capacity still reroll after each completed
  Beacon cycle.

GLOBAL GOVERNMENT CONTROLS

- Ctrl+F5 opens a movable, pinnable Government panel containing all reward
  policies, subsidies, and the live Beacon Network summary.
- The shortcut can be rebound under Beacon & Government in native Controls.
- The global window and building inspector share one renderer, so policy state,
  contract choices, and UI updates remain synchronized. Two-column subsidy
  cards flex for longer future translations.
- Policy and subsidy controls remain unavailable until Government Office I or
  Government Office II has been constructed.

ADDITIONAL BUILDINGS AND TOOLBAR

- Captain's Exchange reuses the vanilla Trading Dock model and research.
- Government Office I and II reuse the matching Captain's Office models and
  research unlocks.
- Relay Beacon, Captain's Exchange, Government Office I, and Government Office
  II use authored default appearances without requiring Recolor or changing
  any prototype or save identity.
- Office I and II share an independent toolbar-only tier group and do not alter
  the vanilla Captain's Office upgrade chain.
- All custom buildings appear in the dedicated Beacon & Government toolbar menu in
  the standard logistics section.
- The current four building prototype IDs remain stable for save compatibility.

OPTIONAL MOD COMPATIBILITY

- Recolor may still apply saved per-building colors over B&G's authored
  defaults. Returning a building to Original restores its authored appearance.
- When Coastal Immigration Beacon is installed, it owns the main Beacon's
  coastal placement, pier, work requirement, preview, inspector, and
  immigration-boat behavior.
- Relay Beacons retain B&G's independent unrestricted 5x5 layout and do not
  inherit Coastal Immigration Beacon's main-Beacon ocean footprint.
- Coastal Immigration Beacon is optional and is not a manifest dependency.

IMPORTANT

This mod can be added to an existing save. Do not remove it from a save after
placing its buildings. Updating must preserve all existing prototype IDs and
the per-save Captain's Currency/network data format.

Author: Underlörd
