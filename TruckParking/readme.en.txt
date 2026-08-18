Truck Parking 1.3.3
===================

Compatible with Captain of Industry v0.8.6c through v0.8.7.

German documentation: readme.txt

Language
--------

MultiLangLib 0.1.0 or newer is required and must be enabled with this mod.
External JSON catalogs cover all 21 languages included with the current game:
Catalan, Chinese (Simplified and Traditional), Czech, Dutch, English,
Estonian, French, German, Hungarian, Italian, Japanese, Korean, Norwegian,
Polish, Portuguese (Brazil), Russian, Spanish, Swedish, Turkish, and
Ukrainian. English remains the fallback for incomplete community catalogs.

Dedicated UI tab and 48 variants
--------------------------------

The mod adds a dedicated Parking tab directly next to Vehicles. It is split
into Transparent and Paved subtabs. Each subtab shows six elements for 1, 2,
3, 4, 6, or 12 parking slots. Like conveyors and other multi-tier buildings,
each element opens the native T1-to-T4 selector on the left:

  - Small (T1):   6 × 3 tiles per slot
  - Medium (T2):  7 × 4 tiles per slot
  - Large (T3):   8 × 5 tiles per slot
  - Super (T4):  10 × 7 tiles per slot

Each size class provides transparent and paved parking areas with 1, 2, 3, 4,
6, or 12 slots, for a total of 48 visible variants. One complete tile row
remains free between adjacent slots for walls or narrow dividers.

Vehicle dimensions are checked before assignment. A vehicle is assigned only
to a slot that fits its loaded prototype. The Super variant is intended
specifically for oversized vehicles from other mods.

Variants and maintenance
------------------------

Transparent:

  - has no surface of its own; the underlying terrain remains visible;
  - base-game or modded surfaces can still be placed underneath;
  - parked vehicles receive an additional 5-percentage-point maintenance
    reduction.

Examples: bare terrain 100% -> 95%; standard concrete 80% -> 75%.

Paved:

  - includes a visible, driveable slab surface;
  - existing surfaces in the construction area are removed and refunded;
  - replacement surfaces cannot be designated underneath;
  - parked vehicles use a total maintenance multiplier of 50%;
  - construction costs approximately 10% less than manually paving the same
    area with concrete slabs.

Concrete slabs for 1 / 2 / 3 / 4 / 6 / 12 slots:

  - Small:   16 / 32 / 48 / 64 / 97 / 194
  - Medium:  25 / 50 / 75 / 100 / 151 / 302
  - Large:   36 / 72 / 108 / 144 / 216 / 432
  - Super:   63 / 126 / 189 / 252 / 378 / 756

Vehicle filter (1:n)
--------------------

Every new parking area has a filter button in its inspector. Any number of
vehicle types can be selected. An empty selection allows all supported ground
vehicles and excludes detected helicopters, airships, and other aircraft by
default. An explicit selection is authoritative, so aircraft can still be
enabled deliberately. The list is generated dynamically from all loaded
prototypes whose runtime type derives from Vehicle, so base-game vehicles and
compatible vehicle mods are supported together.

The reset button next to the filter clears the entire selection, including
saved IDs from vehicle mods that are no longer installed. Filters are saved
and copied with the parking area.

Building assignment
-------------------

The plus/minus button in the parking-area inspector starts the native map
picker. Select any static building that supports the game's vehicle-assignment
interface, including compatible mod buildings. The inspector then shows the
current building name and icon; the icon centers the camera on the building,
and the cancel button removes the link.

A linked parking area accepts only vehicles whose AssignedTo owner is exactly
that building. Multiple parking areas may link to the same building. Those
areas may bypass their local logistics zone for the matching owner, while
clearing the link restores the normal zone check. If every matching slot is
occupied or unreachable, the vehicle keeps the building's native waiting
behavior. Real work, refueling, transfers, replacement, and scrap jobs always
take priority over parking.

The link is stored in save games and copied with the parking-area settings.
Blueprints remap a target included in the same blueprint and discard external
targets, so a pasted blueprint cannot silently attach to an unrelated building.
If the target building is removed, destroyed, or no longer supported, the area
safely behaves as unlinked and the stale link can be cleared in the inspector.

Supported vehicles and behavior
-------------------------------

The manager processes every vehicle registered by the game in
IVehiclesManager.AllVehicles, including trucks, excavators, tree harvesters,
tree planters, and compatible mod vehicles. Helicopters and airships are
detected automatically and stay away from an unfiltered parking area. Select
their exact prototype in the parking-area filter to opt in.

A vehicle is sent to parking only when it is:

  - spawned, enabled, and genuinely idle;
  - free from any real job; only its owner's idle return trip may be replaced;
  - either unassigned for a normal parking area, assigned to the exact building
    linked by that parking area, or eligible for the fuel-station fallback
    described below;
  - not refueling, being scrapped, or being replaced;
  - idle for several consecutive checks;
  - allowed by the filter and small enough for the selected size class.

Trucks and excavators must also be empty. Tree harvesters are checked for
cargo, tree targets, and waiting trucks; tree planters must not have an active
planting operation.

A truck assigned to a fuel station may use an external parking slot after the
station has fully loaded it with the configured fuel product. An empty
assigned truck may park only while its station is disabled or has no stored
fuel. Partly loaded trucks always remain reserved for the native replenishment
cycle, and their return to the station is never replaced with a parking trip.
The mod does not transfer or create fuel, and real refueling, transfer, or
delivery jobs always take priority.

The nearest free slot is reserved before departure. Real work always takes
priority: parking is an interruptible low-priority job. Unlinked parking areas
continue to respect the logistics zone at their position; an explicit building
link can bridge that zone only for vehicles assigned to the matching building.
To avoid a pathfinding spike after loading a large save, at most four new
parking trips start per manager poll; remaining ready vehicles follow on
subsequent polls.

Placement
---------

The construction and rotation pivot remains an invisible, driveable,
surface-free tile at the center of the area. There is no separate yellow
center marker. The marked slots extend forward in the direction of their
arrows. Wide, low selection colliders over the slot markings make both planned
and completed areas easier to select without affecting simulation occupancy.
For reliable pathfinding, keep the visible area level, clear, and accessible
to the intended vehicle type.

After a successful parking trip, the vehicle snaps exactly to the longitudinal
and lateral center of the marked box. Two direction buttons in the parking-area
inspector select alignment with the arrow or exactly 180 degrees against it.
Clicking the active button again leaves direction unrestricted; exact centering
always remains active. The selected state is saved and copied with the parking
area.

Save games and upgrades
-----------------------

The twelve parking areas from the previous release retain their IDs and exact
8 × 5 geometry. After upgrading, they appear as Large (T3), so existing
entities and filters load without migration. The original four-slot parking
area remains available as a hidden legacy prototype.
This hidden legacy variant has no parking-area inspector, so vehicle rotation
remains disabled there. Downgrading to an older mod version is not supported
after the save has been written again with the newer version.

Reservations are rebuilt automatically whenever a save is loaded. Once a
parking area has been saved, the mod must not be removed from that save game.

Changes in 1.3.3
----------------

  - Prevented building links from cancelling native fuel-station return trips.
  - Partly loaded fuel trucks remain reserved for the native replenishment
    cycle; empty trucks park only while their station is unavailable.
  - Fully loaded trucks with the matching fuel may still park externally, and
    queued real jobs remain intact.
  - Added a selectively interruptible parking job so real work advances
    immediately without deleting other entries in the vehicle queue.
  - Extended the compatibility range through Captain of Industry 0.8.7.
  - Existing saves and parking areas remain upgrade-compatible; downgrading
    after saving again with this version is not supported.

Changes in 1.3.2
----------------

  - Fixed the rotation-dependent offset of the exact parking-slot center.
  - Vehicles now snap precisely onto the yellow centerline on parking areas
    rotated by 90, 180, or 270 degrees as well.
  - The save format and stored parking-area settings remain fully compatible.

Changes in 1.3.1
----------------

  - Removed the separate yellow center marker while retaining the invisible,
    driveable placement pivot.
  - Added separate arrow-direction and 180-degree reverse alignment buttons.
  - Kept both direction buttons optional and mutually exclusive; both off
    preserves the previous unrestricted-direction default.
  - Saved and copied the reverse choice without changing the positional save
    format; older enabled settings continue to mean arrow direction.

Changes in 1.3.0
----------------

  - Added optional parking-to-building links through a native world picker.
  - Added a live assignment panel with name, icon, camera jump, change, and
    clear actions.
  - Allowed multiple parking areas per building while matching only vehicles
    assigned to that exact owner.
  - Preserved native waiting when all linked slots are full and kept every
    real work job above parking.
  - Saved and cloned links with blueprint-safe remapping and safe fallback for
    missing or destroyed targets.
  - Let explicit links bridge logistics zones only for their matching owner.
  - Added the new UI and error text to all MultiLangLib catalogs.

Changes in 1.2.0
----------------

  - Migrated all player-facing text to MultiLangLib JSON catalogs and added
    every language shipped with the current game.
  - Added external parking for loaded fuel-station trucks without free fuel.
  - Excluded detected aircraft by default while retaining explicit opt-in.
  - Improved planning-mode selection and made the smaller pivot driveable.
  - Spread bulk parking assignments across polls to protect pathfinding load.
  - Added three custom high-contrast parking icons matching the CoI toolbar.
  - Reordered names so the slot count appears before the size class.
  - Added deterministic build, release-package, and regression checks.

Changes in 1.1.4
----------------

  - Centered parking boxes on the tile grid and corrected the free row between
    neighboring slots to exactly one tile.
  - Vehicles now snap to the exact longitudinal and lateral center of the box.
  - Added optional arrow alignment per parking area; vehicle rotation is
    disabled by default.

Changes in 1.1.2
----------------

  - Grouped parking areas like other multi-tier UI elements.
  - Added dedicated Transparent and Paved subtabs.
  - Added a native T1-to-T4 selector for every slot count.
  - Preserved existing building and toolbar IDs for save compatibility.

Changes in 1.1.0
----------------

  - Added a dedicated Parking tab directly next to Vehicles.
  - Added Small, Medium, Large, and Super size categories.
  - Expanded the selection from 12 to 48 visible parking areas.
  - Extended the manager and 1:n filter from trucks to all Vehicle subclasses.
  - Added safe idle checks for excavators and other working vehicles.
  - Added complete filter reset, including orphaned mod IDs.
  - Added size checks for base-game and mod vehicles.
  - Fixed duplicate manager disposal when leaving the game.
  - Fixed cancellation of stale low-priority parking jobs.
  - Added complete English, German, Ukrainian, Russian, French, Spanish, and
    Portuguese in-game text.

License notice
--------------

The three parking icons are original generated assets packaged in a Unity
AssetBundle. No Captain of Industry artwork is redistributed. The bundle's
open-source container-template attribution and license are included in
THIRD_PARTY_NOTICES.md and licenses/COI-Open-MetallurgyPlus.txt.
