BEACON & GOVERNMENT+
Version 1.1.0b

Beacon & Government+ turns the Beacon into the center of a regional network.
Build Relay Beacons and Government Offices, earn Captain's Currency, purchase
rotating Quick Trades and subsidies, and export researched goods to settlements
through a dedicated Cargo Ship harbor.

INSTALLING AND STARTING

- Extract the Beacon-and-Government-DLC folder from this archive into
  %APPDATA%\Captain of Industry\Mods, then enable Beacon & Government+ in the
  game's mod list.
- A normal Beacon can earn Currency from completed cycles. Research Beacon
  Logistics for Relay Beacons and Captain's Exchanges; Government Office I then
  opens the F5 policy and subsidy interface. Build a Regional Export Yard and
  assign a repaired Cargo Ship before sending settlement aid.
- The Yard starts with every product intake disabled. Enable only the products
  you want trucks to deliver, or choose Request only to follow current demand.
- Existing saves are supported, but a new world experiences the full trust and
  research progression from the start. Existing research stays completed;
  Regional Trust starts at zero because older saves did not track it.

NATIVE WINDOWS

- F3 Trade includes a Regional exports tab with one live card per discovered
  settlement, requested products, shared storage progress and direct fulfillment.
- Its header shows Beacon Network Unity, total regional reputation, Regional
  Trust and Captain's Currency. Hover each pill for its explanation.
- F5 Captain's Office includes Beacon Policies and Regional Subsidies tabs.
- Government Offices and Regional Export Yards retain their detailed inspectors.
- The top HUD, F5 header and custom inspectors share a live, expanding Captain's
  Currency pill. Its hover shows current network Unity and cycle-average reward.
- Left-click the HUD Currency pill for F3 Regional exports; right-click it for
  F5 Regional Subsidies.
- The former Ctrl+F3 and Ctrl+F5 global windows and custom keybinds were removed.
  The native windows are now the permanent global interfaces.

BEACON NETWORK AND REWARDS

- Relay Beacons are optional support sites using the vanilla Beacon model.
- Armed Relays consume 0.25 Unity per month while the primary Beacon operates.
- Government Office I and II can contribute 0.25 and 1.25 Unity per month.
- Government Office I requires 4 workers and 25 kW; Office II requires 12 workers
  and 75 kW. They remain separate placeable toolbar tiers; direct upgrade and
  downgrade are intentionally disabled because decoration replacement is unsafe.
- Regional Export Yards require 4/8/12/16 workers and 250/500/750 kW/1 MW from
  tier I through IV. Yard costs are independent from the assigned Cargo Ship.
- The saved full-cycle Unity average determines the normal reward scale, preventing
  last-second support toggles from granting a full-cycle benefit.
- Completed cycles grant research-gated randomized products and Captain's Currency.
- Village reputation adds up to 25% to generated Beacon rewards.
- Rewards appear in the native arrival window and products go to the Shipyard.

REGIONAL TRUST

- Each Captain's Currency coin actually earned or spent adds one permanent point of
  Regional Trust. Beacon payouts, Quick Trades, subsidies and completed exports
  all count; a transaction has no additional flat trust bonus.
- Trust is saved per world and unlocks research after Government Office I.
  Requirements rise from 100 to 2,000. Power Subsidy Capacity also requires
  five completed village-reputation levels. Its trust requirement starts at 2,000
  for level 1 and rises by 250 per level, reaching 6,750 for level 20. Trust is
  not spent. Already-completed research is not
  removed from existing saves, and older saves start at zero trust.

CAPTAIN'S EXCHANGE AND QUICK TRADES

- Every Captain's Exchange presents three yearly rotating offers.
- Offers use Common, Uncommon, Rare, Epic and Legendary quality tiers.
- Ordinary product offers can be purchased at 1x-5x capacity. Every five combined
  village-reputation levels unlocks the next capacity step; price and goods scale
  linearly.
- Legendary offers are always single purchases and never use capacity multipliers.
- Legendary Cargo Ship, Research and instant Unity offers can appear. Research
  grants assist the active non-space research without overflowing into another node.
- The compact optional alert reads "[Quality] Quick Trades", can be filtered by
  rarity, and opens the first constructed Captain's Exchange when clicked.
- Currency is charged immediately. Product orders prepare for three months, then a
  physical vanilla Cargo Ship follows navigable water to the Exchange and transfers
  the order to the Shipyard.
- Paid orders persist across save/load and wait safely when no Shipyard, berth,
  water route or valid Exchange is currently available.

REGIONAL EXPORT YARDS

- Four tiers are named Regional Export Yard I, II, III and IV.
- They store 250, 500, 750 or 1,000 units of every eligible researched product.
- Tiers I-IV require 4/8/12/16 workers and 250/500/750/1,000 kW respectively.
- Only one Regional Export Yard can be active. It claims one repaired Cargo Ship
  and shares its complete inventory across every discovered settlement.
- Every three years each settlement independently rolls whether it needs aid.
  Base request chance is 50%; combined completed reputation raises it to 75%.
- A request contains three products, each needing a randomized 35-75% minimum.
- Minimum fulfillment sends only the requested minimums and preserves surplus.
  Current stock becomes available at the minimums and sends all currently stored
  requested goods. Maximum waits until every requested storage is full.
- Manual and automatic fulfillment use the same selected mode and trigger.
- Fulfillment requires the Yard's workers and power as well as a docked Cargo
  Ship and the selected order's stored goods. Existing saved Yards reconnect
  their native power consumer when the world loads; rebuilding is not required.
- Each settlement order can be completed once per period. Requests, completion,
  automation settings and export history persist through save/load.
- Newly built Yards start with product intake disabled. Intake can be controlled
  per product or set to Enable All, Disable All, or Request only; the latter
  follows pending settlement requests.
- Intake storage can be sorted by name, rarity, quantity, or category. Show All,
  Hide Full, and Show Demand control visibility without changing truck intake.
- Export history retains the latest 20 successful manual and automatic voyages.
- If a Yard is removed, its stored products enter a saved recovery queue. They
  move to the primary Shipyard when one is available; do not expect the goods to
  appear immediately if there is no primary Shipyard.

BEACON POLICIES

- Immigration Incentives converts rolled Currency into additional refugees while
  preserving product rewards.
- Humanitarian Priority redirects products into support for more refugees while
  preserving Captain's Currency.
- Outbound Resettlement prevents refugees from settling, preserves products and
  adds an independent chance for one additional Coin.
- Supply Compensation prevents refugees from settling, preserves normal Currency
  and raises product rewards by 5%.
- The outward policies are mutually exclusive with each other and both inward
  policies. Immigration Incentives and Humanitarian Priority can operate together.

REGIONAL SUBSIDIES

- Six regional subsidies turn Captain's Currency into temporary emergency support.
- Power Generation imports temporary electrical capacity.
- Computing Capacity leases temporary TFLOPS.
- Water Supply reduces native settlement water demand by 5% per capacity step.
- Wastewater Processing removes 5% of native settlement wastewater per step.
- Garbage Processing removes 5% of native settlement garbage per step.
- Emergency Unity Reserve buys deliberately expensive monthly Unity for crises.
- Farms, factories, ships and industrial waste streams are not altered by the
  settlement-only Water, Wastewater and Garbage contracts.
- Every reputation capacity step applies to all subsidies together. A 5x purchase
  provides five times the quoted capacity at five times the quoted price.
- I-V purchases one to five 24-month terms. Contracts begin next month and retain
  their purchased capacity until expiry.

RESEARCH PROGRESSION

- Every Beacon research node shows its direct prerequisite research in REQUIRES.
  Click a research card to center on that node, even without Procedural Research
  Tree. Trust and reputation milestones are not navigation links. If PRT is
  installed, it still supplies the separate REQUIRED BY section.
- The eleven-node branch follows native progression and keeps all parents mandatory.
- Beacon Logistics unlocks the Relay Beacon and Captain's Exchange.
- Regional Administration unlocks Government Office I and Immigration Incentives.
- Humanitarian Coordination requires Edicts I and unlocks Humanitarian Priority.
- Regional Export Yard I follows the first native Cargo Depot.
- Central Administration unlocks Government Office II, Outbound Resettlement and
  Supply Compensation.
- Regional Export Yard II also unlocks automation for Yard I.
- Regional Power Agreements requires Edicts II and unlocks Power and Water Supply.
- Repeatable Power Subsidy Capacity requires five completed village-reputation
  levels. It strengthens Power Subsidy output without blocking the main branch.
- Regional Waste Management requires Edicts III and unlocks Wastewater and Garbage.
- Regional Export Yard III follows, then Computing Capacity Agreements unlocks
  Computing and Emergency Unity, followed by Regional Export Yard IV.
- Yard III unlocks automation for Yard II, and Yard IV unlocks automation for
  Yard III. Yard IV automation is intentionally reserved for later progression.

LOCALIZATION AND INTERFACE

- All custom text is included for the game's 21 supported languages.
- English is the source language for editable JSON translation keys. The 21
  locale tables have matching keys and formatting tokens; players can customize
  wording in the translations folder without rebuilding the mod.
- Native F3/F5 menu tabs retain Captain's-gold selection styling. Government
  Office and Regional Export Yard building-inspector tabs use the neutral native
  building presentation.
- Long-language layouts use compact two-column cards, shortened export rows,
  hover explanations and fixed native-window sizing.
- Village names can be customized with supported rich-text color tags.

SAVE AND COMPATIBILITY NOTES

- The mod can be added to an existing save but should not be removed after placing
  its buildings.
- Updating preserves existing prototype IDs, Currency, deliveries, village orders,
  export storage, policies and subsidy state.
- Newly inserted research appears unresearched in existing saves. Complete the node
  to unlock future purchases or construction; already placed buildings and saved
  active contracts remain intact.
- Kayser's optional Pre-Industrial Era Tier-0 Lighthouse keeps its own refugee
  and supply rewards; B&G rewards and Currency begin at the normal Beacon.
- Compatible with optional Coastal Immigration Beacon and Shipping++ handling.
- B&G's transient delivery ships are removed before save serialization and safely
  reconstructed from durable pending-order data after loading.

Author: X-Mag-X
