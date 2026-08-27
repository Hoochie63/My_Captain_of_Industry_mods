# Cargo Helicopter changelog

## 0.19.10 (GAME-0.8.7-COMPATIBILITY)

- Added verified support for Captain of Industry **0.8.7** and raised the CoI Hub compatibility ceiling to `0.8.7`.
  The release compiles with zero warnings against public Steam build **24719404 / v0.8.7a**.
- Audited every active Harmony target and reflected private member used by flight, pathfinding, cargo transfer,
  heliport production, landing and scrapping. All active targets are still present in the 0.8.7 game assemblies.
- Removed an unused experimental real-product re-render path that referenced
  `FlatBedAttachmentMb.m_productsToRenderDynamic`, a private field removed by the new product renderer. The shipped
  cosmetic sling/container path remains unchanged.
- Replaced frame-count and process-uptime watchdogs with one monotonic active-play clock. Production, refuelling,
  scrapping, winch and C-yard recovery timers now stop while the game is paused, unfocused or the computer is asleep,
  preventing false timeouts immediately after resume and making their limits independent of render frame rate.
- Serialized heliport and helicopter destruction cleanup through the game's synchronized update, with bounded retries
  and duplicate suppression. Demolishing a producing or final heliport can no longer race the dispatcher, strand an
  emerge animation, retain a stale yard lease or leave recycled vehicle IDs in refuel/scrap/API state.
- Added a scheduled GitHub Action that mirrors new public CoI Hub forum reports into deduplicated GitHub issues and
  comments. The importer uses no CoI Hub credentials, sanitizes untrusted forum content and limits each run to a
  bounded batch for safe recovery after outages.
- Added a maintainer helper that verifies a clean, version-matched `main`, builds the release archive and checksum,
  and creates a draft GitHub Release. It never publishes the release or uploads to CoI Hub automatically.
- Kept the existing Unity 6000.0.66f1 AssetBundles: Captain of Industry 0.8.7a still uses Unity 6000.3.19f1 and loads
  those bundles. The release builder now rejects any unexpected change to the T2 `open_heliport` bundle hash. No
  models, balancing, prototype IDs or save data changed.

## 0.19.9 (TERRAIN-ANCHORED-HELIPORTS)

- Fixed the actual Open Heliport placement defect shown in the tester screenshot. Previously only the building origin
  followed terrain; the remaining 232 x 114 m T2 footprint had no terrain-height contract, so a platform spanning
  uneven ground could legally float over a valley on one side and pass through a hill on the other.
- Every occupied T1/T2 footprint tile now declares terrain surface height zero relative to the building. Placement and
  construction therefore use one foundation plane across the complete model. A dedicated placement validator also
  rejects the preview until every terrain vertex is within the ground-plane tolerance; instant-build tools can no
  longer turn a terrain-blocked designation into a finished floating/submerged platform. No hardened-floor surface
  is painted, and no 3D model or AssetBundle was changed.
- Existing incorrectly placed platforms are not teleported; dismantle and rebuild them after updating.
- Retains the mandatory material-production and landed-scrapping fixes from 0.19.8.
- Built for Captain of Industry **0.8.6c / Update 4.2**.
- COI Hub compatibility metadata uses `0.8.6` because the Hub currently treats the public `0.8.6c` hotfix suffix
  as a version newer than its known `0.8.6` release; runtime verification remains 0.8.6c.

## 0.19.8 (TESTER-FEEDBACK / PRODUCTION-AND-SCRAP)

- Audited object-overlap placement. Current T1/T2 layouts occupy every tile of a footprint slightly larger than each
  complete model (T2: 232 x 114 m layout around a 228.6 x 110.6 m mesh), so walls, buildings and other occupied tiles
  are rejected. Terrain-height anchoring itself was corrected separately in 0.19.9 after an exact screenshot exposed
  the distinct uneven-ground case.
- Removed the Unity quick-build purchase path from both Open Heliports. Helicopter production now always consumes
  its existing per-tier Vehicle Parts/Rubber inputs and construction time; the stock depot continues to expose
  material inputs and stores recovered helicopter/fuel products in its outputs after scrapping.
- Fixed Scrap Vehicle and replacement behaviour for helicopters. After the stock return route reaches the Open
  Heliport queue, the aircraft now descends to the deck, confirms touchdown, keeps its rotors at ground idle, rolls
  to the despawn position, and only then is removed. The fake 100-keyframe door wait on the door-less deck was
  reduced to one keyframe, removing the unexplained 20-40 second hover/despawn delay.
- Retains 0.19.7's active direct-air route and strongly damped sling fixes for the tester's mountain-flight and
  container-swing reports. No 3D models or AssetBundles were changed in this release.
- Built for Captain of Industry **0.8.6c / Update 4.2**.

## 0.19.7 (ACTIVE-DIRECT-FLIGHT / SLING-STABILITY)

- Fixed the remaining car-like turns on old and already-active routes. Direct flight is now enforced inside the
  synchronized active-route update, not only when a new pathfinding result arrives. A loaded terrain/road segment,
  its road-driving trajectory and queued waypoints are cleared, then the hidden vehicle receives the final goal as
  one long-distance, obstacle-independent target. Existing saves therefore switch to a straight bearing without
  waiting for the job to finish or for a new route to be calculated.
- The legacy terrain-segment fallback now also targets the final goal directly; it can no longer walk along the
  next saved ground waypoint.
- Tamed the slung-load motion: physical pitch/roll is limited to roughly 4.6-6.9 degrees, angular speed and
  acceleration impulses are clamped, damping is substantially stronger, and the extra synthetic wobble is now
  below one degree instead of stacking another 6-14 degrees on top of the pendulum.
- Built for the current public Captain of Industry **0.8.6c / Update 4.2**.

## 0.19.6 (DIRECT-AIR-ROUTING / GAME-0.8.6C)

- Helicopter X/Z movement is now genuinely point-to-point. The game still selects and validates the logistics
  destination, but after a successful search the mod discards every ground, road and traffic waypoint and keeps
  one direct air leg to the selected goal. The existing bounded-hop driver follows that same bearing all the way,
  so mountains, cliffs, water, buildings and roads can no longer make the aircraft snake like a truck.
- Terrain remains fully relevant to vertical safety only: the altitude controller scans the new direct line at
  6 m intervals, starts climbing before a ridge, keeps roof/terrain clearance under the belly, and descends smoothly
  after the crest. It never feeds those samples back into the horizontal route.
- Rebuilt and API-verified against the current public Captain of Industry **0.8.6c / Update 4.2** installation.
  The manifest now explicitly verifies through `0.8.6c`; the direct-flight API is also present in 0.8.6a/b.

## 0.19.5 (MOUNTAIN-REGRESSION-FIX)

- Fixed the 0.19.4 mountain regression: removing the ground under the helicopter from the altitude
  target pushed all terrain reaction into the clearance clamp, and the 11 m/s climb cap could not
  keep up with steep slopes at cruise speed — the hard emergency backstop fired every few meters.
  The ground beneath the aircraft is part of the (smoothed) altitude target again, so climbs start
  early, and when clearance runs out the climb rate boosts to 25 m/s instead of snapping.
  The ridge "shelf" behavior from 0.19.4 is kept: fast follow uphill, slow shallow descent.

## 0.19.4 (MOUNTAIN-FLIGHT-SMOOTHNESS)

- Fixed jerky flight over mountains. The altitude target no longer follows the ground directly
  under the helicopter: in cruise it looks ahead along the route only, so the aircraft holds a
  level "shelf" over a ridge instead of rolling down every slope like a car.
- Emergency altitude snaps now trigger only when the helicopter physically cannot climb in time
  (climb rate vs distance to the obstacle), not merely because terrain rises within 100 m —
  ridgeline flicker can no longer yank the aircraft upward.
- Descent past a ridge is much shallower far from the destination (2.5 m/s, ramping to 6 m/s on
  final approach) with a shorter peak-hold, so the helicopter glides down instead of nosing over.
- Route obstacle scan is anti-aliased: 6 m sampling step (was 12 m) and rescanned every 0.25 s
  instead of every frame.
- Nose pitch from vertical speed limited to ±3° so gentle altitude corrections don't bob the nose.
- Safety: the under-belly clearance clamp and backstop now consider building roofs as well as
  terrain, so cruise without terrain-following is strictly safer than before.

## 0.19.3 (FLIGHT-SMOOTHNESS)

- Fixed the jerky stop-start crawl after every load/unload. While the winch is still reeling the
  sling in, the helicopter now holds its next job in a stable hover instead of fighting the game
  with per-tick StopDriving calls. Winch retract is faster (14/16/18 m/s), and the hold has a
  20-second failsafe so an aircraft can never be stuck held forever.
- Smoothed the cruise altitude target: descending to a lower route base is now low-pass filtered
  (climbs stay instant for obstacle safety), and the hard clearance clamp at obstacle edges is a
  soft ramp instead of an instant pop. Altitude-traffic lanes no longer collapse and reappear
  during loading, taxi and cargo drops.
- Tamed the weather wobble: crosswind crab reduced from 13° to 6.5°, gust sway roughly halved,
  and gusts no longer speed up at 2x/3x game speed. Heading follow is more responsive
  (120°/s), so the nose no longer lags in steps on route corners.
- Added visual smoothing (SmoothDamp, ~0.1 s) between the simulated and rendered altitude,
  removing vertical steps at high game speed or low FPS. Scripted sequences (assembly emerge,
  equipment yard, taxi) are unaffected.
- Long helicopter routes now use 22-tile pathfinding hops instead of 16 (engine assert limit is
  24), reducing micro-stops on long flights by about a third.
- Steady cruise flight: the altitude target no longer ratchets up and down over narrow buildings
  (rate-limited rise at 25 m/s with an emergency snap only below 100 m, hysteresis and a 2 s
  peak-hold before descending). The helicopter holds a level attitude in straight flight — banking
  now comes from turn rate (coordinated turn) and pitch from vertical speed, both zero on a
  straight leg. Crosswind crab is steady instead of breathing with gusts, and the hover bob and
  aero jitter fade out above walking speed. Rotor sound no longer calls out every altitude wobble.

## 0.19.1 (UPDATE-4.2-CHEAT-COMPAT)

- Set the COI Hub compatibility target to the public **0.8.6** release. The mod was also locally
  tested on the game's `0.8.6a / Update 4 build 610` hotfix identifier.
- Updated the bundled Harmony runtime from 2.2.2 to 2.4.2, matching Cheat++ 1.3.0. Cargo Helicopter
  can no longer load an older process-wide Harmony assembly before Cheat++ and silently downgrade it.
- Declared Cheat++ as an optional dependency so, when both mods are enabled, the game initializes
  Cheat++ first without making it mandatory for Cargo Helicopter users.
- Added an optional UI compatibility watchdog. If Cheat++ is active but its one-shot delayed
  C/Overlord toolbar insertion ran before the Update 4.2 HUD was ready, Cargo Helicopter asks
  Cheat++ to retry its own idempotent installer every eight seconds until the button is registered.

## 0.19.0 (UPDATE-4.2-PRODUCTION-FIX)

- Added verified support for Captain of Industry 0.8.6a / Update 4.2 and its Unity 6000.3.19f1
  runtime. The manifest now accepts 0.8.6 while retaining 0.8.5 as the minimum supported version.
- Fixed the normal (non-instant) production deadlock. A completed helicopter was marked `JobHeld`
  before the stock `SpawnJob` could execute its initial `Vehicle.Spawn`, leaving the first aircraft
  invisible and eventually filling every assembly lane. Heliports now use the game's valid direct-spawn
  path after the normal material, time and queue checks, then run the authored vertical emerge.
- Added bounded production recovery: a stripped direct-spawn patch releases the stock spawn job after
  10 seconds, a missing Unity view bypasses emerge after 20 seconds, and an emerge stuck for 60 seconds
  is safely handed to the regular stand dispatcher. A broken animation can no longer reserve a lane forever.
- Multiple heliports no longer amplify the production stall: each completed helicopter keeps its producing
  depot and lane through spawn, emerge and first-flight equipment pickup.
- Enabled the game's replacement workflow from an ordinary vehicle to a helicopter at an Open Heliport.
  Normal trucks remain hidden from the heliport construction menu.

## 0.18.0 (CONFIGURABLE-ECONOMY)

- EDITABLE ECONOMY via the in-game mod settings (config.json): fuel consumption multiplier, cargo capacity
  multiplier, build time multiplier, and the power draw of both heliports. Change them in the mod settings and
  reload the save — the values are baked into the vehicle/building prototypes, so a reload applies them.
- Economy rebalance (new defaults, all editable):
  - Fuel: tanks resized for ~22 minutes of endurance (Heli I 26 / II 50 / III 100 diesel) so refuel trips are
    far less frequent. Burn rate stays ~2x a truck of similar cargo — thirsty but fair, not punishing.
  - Build time: 40 / 55 / 75 s (was 30 / 45 / 60) so a helicopter costs a bit more effort to field than the
    equivalent truck, keeping the vanilla balance intact.
  - Capacity unchanged (40 / 90 / 180).
- Tier-differentiated speed: helicopters are now faster, and each tier is meaningfully quicker than the last —
  32 / 38 / 44 m/s (was a flat-ish 28 / 32 / 36). They remain the fastest logistics option and still fly
  straight over any terrain. The departure-merge intercept ceiling was raised to match so the visual still
  tracks the route perfectly.

## 0.17.0 (LEGACY-SAVE-SAFE)

Also in this release - THE PATHFINDER ASSERT STORM IS FIXED AT THE SOURCE: helicopter paths (slope/clearance
ignored) get simplified into single waypoints beyond the engine's 24-tile direct-drive bound, which produced
thousands of per-tick assert stack traces ("Value of Fix64 ... expected < 576"), a visible sim slowdown and
broken route progress whenever helicopters flew long legs. Helicopters now walk far waypoints in bounded
~16-tile hops toward the same point, satisfying the engine's own contract; ground vehicles are untouched.

SAFE UPGRADE FROM THE PUBLIC 0.8.x RELEASES. The old hangar-style "HeliportComplex" building prototype is
registered again (same id, same 36x32 layout, same spawn tiles, same prefab from the retained heliport_complex
AssetBundle), so a save that contains one LOADS instead of failing with an unknown prototype. The building is
deprecated: it no longer builds or services helicopters (production and dispatch belong to the Open Heliports),
carries a description telling the player to replace it with an Open Heliport T1/T2, and has no research unlock.
Helicopter/attachment/vehicle-group prototype ids never changed between 0.8.1 and now, so vehicles, cargo and
research state migrate untouched. A prototype referenced by players' saves must stay registered in every future
version - remove it and their saves die.

## 0.16.3 (NO-POINTLESS-MOVES)

- No more "places the container and immediately takes it back": a task that arrives while the aircraft still
  carries its attachment ON THE HOOK (return not yet handed off - hold queue, approach, or lowering before
  ground contact) now departs DIRECTLY into the live route when the required attachment kind matches. The
  unused return leases are released in place; only a kind mismatch (container vs tank/logs) still visits the
  yard to swap.
- Parking stands free the moment a departure's pickup starts, not when its route merge completes: arriving
  aircraft can now land on the seats of a departing group immediately instead of waiting for the whole group
  to leave the area. Cancellation paths re-reserve a stand when they need one, as before.
- Crane steadiness: wind push and aero wobble on the hull are damped to ~10% while the winch is placing or
  collecting a load - the hoist point no longer wanders every frame, which was the visible jitter during
  container placement on the pad.

## 0.16.2 (RECALL-CHURN-HARDENED)

Second audit pass (the two reviewers that failed the first pass, re-run) confirmed five more, all applied.
Two of them re-opened the "fleet hangs" class and were regressions in my own 0.15.1 recovery logic:

- A single empty-sequence gap tick between two re-issued jobs used to WIPE the whole recovery record (anchors,
  loiter clocks, load-recovery flag). A job-churn loop (job fails, re-issued within ~20 ticks) therefore reset
  every watchdog each cycle and stayed invisible to all recovery. The record is now aged out over the
  idle-confirm window instead of deleted on the first empty tick.
- The true-job loiter-clock reset (added in 0.15.1) fired on EVERY job-object change - but the stock scheduler
  makes a new job object per issue, so a broken route re-issuing an unreachable target in place reset the
  recall clocks forever. The reset now also requires the aircraft to have physically moved 40 m since the last
  reset, so a genuine delivery leg still resets it but in-place churn cannot.
- Spread-hold now assigns each waiter a STABLE distinct slot (lowest free index among same-column waiters)
  instead of a queue-position snapshot, which could hand two aircraft the same hold point after a proximity
  admission removed a middle entry.
- Transfer ground-gate cap raised 200 -> 400 ticks: at high game speed with low FPS the winch integrates
  fewer game-seconds per sim tick, and a legitimate deep drop could otherwise trip the mid-air-transfer cap.
- HasTrueJob (called per heli per base per tick) now uses a compiled delegate instead of reflective GetValue,
  removing the per-call bool boxing garbage that scaled with fleet x base count.

## 0.16.1 (CAPACITY-AUDIT-POLISH)

Adversarial audit of the 0.16.0 capacity redesign confirmed six refinements; all applied:

- Terrain settling now keys on how long capacity has been CONTINUOUSLY absent (own timer), not on total idle
  time, and climbing out requires capacity to be back for ~3 s - a one-frame full-stands dip can no longer drop
  a long-idle aircraft onto the grass, and a flickering free stand cannot yo-yo a grounded overflow fleet.
- The capacity snapshot the terrain gate reads is now RECRUITABLE capacity (free stands AND fleet-ceiling room
  AND a leasable C-slot), so it can never wait for seats nobody could actually consume.
- The blocked-return counter resets whenever the aircraft stops being a blocked returner (takes a job, adopts a
  stand), so migration measures one continuous ~90 s blockage instead of accumulating short episodes.
- Migration now requires the target to actually be landable (leasable C-slot probe, mirroring the demolition
  path) and picks the NEAREST eligible base - no more rebinding into a base that immediately re-blocks.
- Stand capacity republication is monotonic (no clear-then-add) and shrink-safe against the SIM-thread build
  gate - demolition can no longer let one extra airframe slip past the global cap.
- The loaded-fleet stationary watchdog is winch-aware and its transfer threshold raised to 300 ticks: a deep
  65 m hoist cycle at the slower winch no longer trips "no meaningful SIM progress" mid-delivery.

## 0.16.0 (FLEET-CAPACITY-HONESTY)

Fleet capacity redesign - fixes the overproduced fleet that could never land:

- The 0.15.1 build gate shared the unbound-fleet reservation across bases, which let every base under-count
  everyone else's aircraft: the world produced MORE helicopters than parking stands exist. The gate is now
  exact and two-level: the WORLD may never contain more airframes than total stands (counts every spawned
  helicopter, immune to binding states), and each base's own bound fleet may not exceed its own stands.
- SOFT home binding: home is now a preference, not a life sentence. An aircraft whose return stays blocked for
  ~90 s (overbooked or shrunken base) migrates to any base with real fleet+stand capacity, with a log line.
- ZERO-capacity graceful landing: when bases exist but not a single free stand remains on the map (an already
  overproduced save), an idle aircraft sets itself down on open terrain after ~25 s instead of hovering
  forever, and lifts off again the moment capacity appears. An existing broken save therefore self-heals:
  excess aircraft either migrate to free seats or wait parked on the ground.

Animation polish (takeoff / landing / container placement):
- Winch pays out at 6.5 m/s and reels at 9 (was 9/12) - the cable no longer fires at the ground.
- Stand/terrain descents slowed ~25% and takeoff climb softened (7.5/6.5 m/s, gentler acceleration).
- Final descent onto the stand takes 2.2-6 s scaled by height (was 1.65-4.5), and the post-touchdown settle
  is 0.35 s, so suspension compression reads before the handoff.

Cleanup: removed the dead s_emptyTailTicks registry, two unused sequence-inspection helpers and the never-used
YardMoveTimeout constant.

## 0.15.3 (NEAREST-LANDS-FIRST)

- Landing admission is now independent of recruit order. The physical C-column token used to be granted the
  moment an aircraft was recruited for return - even from across the map - so every nearer finished aircraft
  queued on the same column hovered until the far one completed its whole inbound flight (worst case: one
  unfinished aircraft made the rest of its column wait indefinitely). The token is now granted to the first
  waiter that is actually NEAR the base (within 150 m of its column); far waiters keep their queue position
  without blocking anyone, and the per-column FIFO still applies among aircraft that are actually present.
  T1 still lands up to 5 concurrently and T2 up to 10 - one per physical column - unchanged.

## 0.15.2 (HOME-OWNED-COUNTERS)

THE multi-heliport hang, found via the new fleet heartbeat (92 idle aircraft, zero tracker entries, zero
return attempts, total silence): with two or more heliports in the world, every NON-home depot's dispatch
prefix executed `s_idleTicks.Remove(id)` for aircraft bound to another base - erasing, every single tick, the
idle-confirmation counter the home prefix had just incremented. The 72-tick idle debounce could therefore
never complete and an idle aircraft was NEVER recalled: the whole fleet hung mid-air indefinitely. Every
earlier test ran on a single-base world, which is why no session before this one reproduced it. This defect
predates 0.14.x (present in 0.13.4 and earlier).

- Foreign prefixes no longer touch another base's lifecycle counters; they return without side effects.
- Loiter/wander anchors are now also reset by TRUE-job identity changes, so a helicopter legitimately
  shuttling between two points closer than the wander disc is never recalled mid-work (helper-job churn
  cannot fake this - NavigateTo/GetUnstuck are not true jobs).

## 0.15.1 (WANDER-RECALL-HEARTBEAT)

Diagnosed from the live 0.15.0 session log (4.4 MB): after the player demolished every worksite at once, the
game's pathfinder produced terrain segments beyond its own 24-tile bound for dozens of helicopter routes
(`Value of Fix64 ... expected < 576` + `Driving target ... did you mean path finding` every few ticks for six
minutes). The affected aircraft wandered in wide loops around the fallback receiver (the port), which reset the
30 m loiter anchor forever - so no recall ever fired and the whole fleet looked permanently frozen.

- WIDE loiter recall: any unmanaged busy aircraft that stays inside a 120 m disc (wandering, not just hovering)
  is recalled after ~90 s (loaded) / ~4 min (live) regardless of how much it locally moves.
- CANNOT-DELIVER fast recall: when the game itself flags the truck's cargo as undeliverable (mass demolition of
  receivers), the aircraft is recalled after ~60 s instead of orbiting a dead destination.
- FLEET HEARTBEAT: one log line per depot per ~minute with every FSM state count, unmanaged busy/idle split,
  tracker/recall/blocked-return counters and stand occupancy. A silent fleet is now diagnosable from the log.
- The dispatcher prefix and the per-frame drivers no longer die silently: the one-shot warning latch became
  rate-limited logging, and each frame subsystem (yard, flight, emerges, watchdog...) is isolated so one
  exception cannot freeze everything else - "all helicopters stop until restart" can no longer happen silently.
- Hold-queue spreading: every waiter of a C-column used to receive the SAME holding point, parking several
  aircraft inside each other's rotors; later arrivals now hold 12 m further out and slightly higher, per queue
  position.
- Open Heliport T1 down to 100 Construction Parts + 20 Iron and 100 kW - genuinely start-of-game buildable.

## 0.15.0 (FULL-AUDIT-SWEEP)

Result of a full multi-agent audit of every source file with adversarial code-grounded verification (41 raw
findings, 18 confirmed after the 0.14.x fixes). Root causes of both remaining user-visible failures found.

Return speed / motion honesty:
- Scripted yard/recall legs are now SPEED-true. `LegDuration` clamped every leg to at most 6.5 seconds, so an
  aircraft recalled from 500 m away flew home at ~77 m/s average (~145 m/s quintic peak) - the "unreal return
  speed". Legs now fly at 16 m/s near the pad, up to 26 m/s on long recalls, however long that takes.
- Return/hold approaches DESCEND to deck+10..20 m while flying in, instead of keeping inherited cruise height
  over the C-place and paying out tens of metres of cable (above deck+~72 the drop exceeded the 60 m cable
  entirely and could never ground).

The YardLowering hang (helicopters frozen with diagonal cables - the recorded video):
- ROOT CAUSE: the cargo-drop render hook pinned the sling onto the C-slot from ANY distance once 2 m of cable
  was out, and the displaced sling was then republished as the attachment/root offset sample. The dispatcher's
  next yard leg computed target = slot - (slot - root) = the aircraft's own position: the leg "arrived"
  instantly wherever the aircraft was, the winch ran there, and every recovery re-confirmed the wrong spot.
  The centring now only applies within 6 m of the slot, the published offset has the centring shift removed,
  and StartYardMove rejects offsets beyond plausible sling geometry (falls back to targeting the root).
- The winch grounding test was purely vertical; with a dispatcher drop target it now also requires the
  aircraft to actually be over the slot, so a stray aircraft can no longer materialise a container onto a
  C-place from across the map (or hang paying out cable at the wrong spot).
- The 0.14.1 YardLowering re-submit reset the state timer through YardApproach, silently defeating the
  1200-tick abort; restarts are now counted and the aircraft is released after 3 failed attempts.
- View destruction of a still-live vehicle no longer wipes the per-vehicle runtime state (that wipe removed
  s_yardActive before the dead-view sweep could see it, making the death silent again); the sweep now also
  detects REPLACED views (fresh HeliState with no driver). Phase 3 (align over stand) got the timeout the
  YardMoveTimeout constant always promised; stale exit-ready/merged handshakes are cleared at plan activation;
  re-activated legs start from the last reported visual pose instead of the terrain-projected SIM transform.
- The departure-merge intercept now decays its target-velocity estimate when the route stalls; previously the
  visual overshot a stopped route point by ~20-30 m at full speed and the merge gate became unreachable until
  the route moved again.

Job/logistics integration:
- Empty-route cancellation (CurrentJob==null while navigating) is debounced by 4 s - the same scheduler-gap
  family as the departure-merge fix; healthy sequences with queued follow-ups are no longer reset.
- The stalled-transfer watchdog was raised 45s -> 90s and no longer counts time while the winch is still
  paying out; it used to cancel legitimately slow deliveries, looping helicopters against full receivers.
- A refuel "land here" clearance leaked when the refuel job was cancelled mid-approach, then forced a full
  landing (plus a 12 s transfer stall) at EVERY later stop. Clearances are now stamped per job tick and swept
  3 s after the job dies; refuel landings also never override dispatcher-managed choreography anymore.
- Storage/ore-sorting queue bypass is now keyed on the prototype instead of render-populated state: a heli
  could enter a storage's one-vehicle queue during the post-load warm-up and its stale entry then starved
  every truck behind it forever.
- The transfer ground-gate re-arms per product and warns when its 120-tick cap forces a mid-air transfer.

Yard/lease protocol:
- ReflowColumn seats the bottom visual on the AUTHORED deck; an upper container no longer floats in mid-air
  after the lease below it is removed (with later parks stacking on top of the floater).
- AllocSlot refuses to lease before the depot's markers register (T1 slots computed with the T2 column count
  in that window, splitting physical columns and overlapping containers).
- A failed unpark commit can no longer freeze a whole column forever: per-item error handling clears the
  pending-removal token and raises the FSM-visible failure flag.
- Templates with dead materials are rejected (previously an INVISIBLE attachment could be committed as a
  stable occupied slot); atomic handoffs wait ~15 s instead of 60 s for a missing sling view; a swapped lease
  no longer inherits the previous owner's build-failure flag.
- Direct sling/payload visibility commands now supersede an older queued visual handoff (a stale queued
  command could re-hide a flying helicopter's cargo indefinitely).

Threading / lifecycle:
- Depot demolition and vehicle destruction notifications are queued and drained inside the synchronized
  dispatch window instead of mutating plain dictionaries from the SIM teardown thread while the main thread
  enumerates them (crash/corruption risk on demolish-at-speed).
- Production patches no longer read Unity Transforms from the SIM thread (deck offset is cached at resolve).
- Production fails CLOSED while assembly-line markers are unresolved - a completed aircraft can no longer
  spawn at the stock depot door, bypassing the first-flight contract.
- Destroyed vehicles release their stand/column/lease synchronously and no longer leak idle/job caches onto
  recycled entity ids; a demolished base clears JobHeld for production-owned aircraft that the FSM had not
  adopted yet (they were permanently unschedulable AND exempt from the game's own stuck-vehicle rescue).
- The in-sync drain is re-bound per loaded session (it silently pointed at the previous session's dead
  GameLoopEvents object from the second load onward). The post-load build gate shares the unbound-fleet
  reservation across live bases instead of holding EVERY heliport's production for the whole world's fleet.
- PickupAscending releases the column token when it cannot use it (mutual block with a returning aircraft
  that owned the column's top attachment); PickupHandoff reacts to a timed-out main-thread handoff.

## 0.14.1 (ARRIVAL-WINCH-WATCHDOG)

Follow-up found by running 0.14.0 on the clean save: parallel returns across C01-C05 worked, but three
aircraft returning from mid-flight save positions hung the full 2-minute timeout in `YardLowering`.

- A vehicle whose Unity view dies while a scripted yard leg owns its pose left `s_yardActive` set with no
  winch integrator running; the dispatcher then waited on `IsCargoGrounded`/`IsSlingRetracted` forever. Dead
  views are now swept every frame and turn into an explicit yard-move failure.
- `YardLowering`, `PickupLowering` and `PickupReeling` now react to a failed yard driver (previously only the
  approach/landing states checked). `PickupHandoff` reacts to the bounded main-thread handoff giving up.
- If cargo still is not grounded after 300 ticks in `YardLowering`, the flown leg is re-submitted in place
  (continuous redirect, no pose snap) with a one-line winch/lease state dump for attribution.

## 0.14.0 (PARALLEL-PICKUP-HONEST-MERGE)

- Fixed the T1/T2 departure serialization root cause: once an aircraft reserved its departure column, the per-tick
  top-of-column stability re-check permanently blocked the attachment kind replacement it was itself performing
  (the replacement legitimately bumps the lease generation). Any job needing a tank/logs attachment therefore hit
  the 2-minute `stuck in PreparingPickup` release loop and departures trickled out one at a time. The stability
  gate now runs only until the reservation is acquired.
- Fixed the false `job vanished during departure merge`: `HasTrueJob` legitimately flips off between the internal
  jobs of a live sequence and in the 17-23-tick scheduler window after a completed delivery. The departure merge
  now cancels only after 60 consecutive idle ticks with a genuinely empty job queue, so aircraft with live work no
  longer fly home, land and immediately take off again.
- Helicopter cruise speeds rebalanced from 3.0/3.6/4.2 to 1.4/1.6/1.8 tiles per tick (vanilla trucks: 0.9-1.0).
  The old values were 60-84 m/s, which both looked wrong and made the departure-route intercept (capped at 30 m/s)
  physically unable to catch a cruising SIM route - that unfinished chase was the second source of vanished-job
  cancellations. The intercept ceiling is now 42 m/s, above the fastest tier.
- Mass-fleet save recovery: added anchor-based loiter detection. A saved route that CIRCLES an unreachable target
  moves a few metres every tick and reset the old stationary watchdog forever (the 0.13.4 log shows zero recalls
  for 250 hovering aircraft). Any unmanaged aircraft that stays inside a 30 m disc is now dumped once (full SIM
  job/cargo/flag state, `stuck-dump` lines) after ~30 s and recalled after ~60 s (saved) / ~3 min (live jobs).
- Recall/return attempts blocked on stand or C-slot capacity are no longer silent; they log the reason once per
  minute per aircraft.
- Staged attachment visuals whose main-thread park/handoff work item was silently invalidated by concurrent lease
  churn are re-queued after ~15 s instead of waiting forever (previously the FSM waited on one-shot request flags
  until the 2-minute timeout). Applies to pickup preparation, arrivals, post-load restore and factory equipment.
- Early-game economy: Open Heliport T1 now costs 180 Construction Parts + 40 Iron and consumes 180 kW (was 260
  Vehicle Parts + 110 Steel, 450 kW) and unlocks with the first vehicle research, same node as Cargo Helicopter I.
  T2 costs 300 Vehicle Parts + 120 Steel at 500 kW (was 500/200, 900 kW) and unlocks with Vehicle Assembly II.
- Added `still preparing pickup` periodic diagnostics (column owner, top-of-column owner/state, full slot lease
  state) so any remaining contention is attributable from a single log line.

## 0.13.4 (ATOMIC-SAVED-FLEET-RECALL)

- Fixed the serialized sequence shape that escaped every previous recovery branch: `HasTrueJob=false` with a live
  `NavigateTo` / queue / unstuck job and `IsNavigating=true`. Busy jobs and non-true navigation tails now share one
  authoritative SIM-progress watchdog instead of the latter returning forever.
- Route progress now requires two metres of real movement. Centimetre-scale pathfinder jitter and helper-job churn no
  longer reset the recovery clock; saved non-true tails also have a bounded load grace so a crawling legacy route
  cannot remain scattered over the map indefinitely. Recovery remains staggered by vehicle id.
- Recovery is now atomic: before cancelling a stale sequence the aircraft is latched for recall and job acquisition
  is held. Once the old sequence is empty it bypasses the ordinary idle debounce, reserves its own C-place and stand,
  and immediately enters the normal visible return/landing FIFO.
- Patched Truck's private `tryGetJob` scheduler entry point in addition to `doJob`. The stock scheduler can no longer
  assign a replacement delivery between cancellation and heliport admission, which caused the repeated container
  drop -> hover -> new job loop.
- A job which declines normal cancellation stays inside the recall latch and is finalized after a bounded grace;
  movement or an internal job-object swap can no longer silently disarm that stage. Player scrap/replacement still
  overrides recall and releases all holds immediately.
- Legacy home selection waits one render frame for every T1/T2 marker set to register, then restores aircraft found
  on authored stands before using the nearest-capacity fallback. Prefix order can no longer bind a saved parked
  helicopter to the first base which happened to initialize.

## 0.13.3 (SERIALIZED-FLIGHT-RECOVERY)

- Fixed the actual legacy-save freeze: older scripted yard flights persisted `Entity.IsPaused`, while their runtime
  owner tables were intentionally rebuilt empty after loading. Unbound saved helicopters are now unpaused from the
  synchronized simulation section, independently of renderers, camera visibility and game-speed multiplier.
- Reordered load recovery so every in-flight helicopter receives one capacity-checked home before the busy-job gate.
  A true job can no longer keep an aircraft permanently outside all T1/T2 dispatchers merely because the save was
  loaded while it was flying.
- Added a simulation-position and exact-current-job watchdog for loaded busy routes. Only an unchanged route is reset;
  moving aircraft keep their work, and real pickup/delivery transfers receive a longer safety window. Recovery is
  staggered by vehicle id so hundreds of old aircraft do not all cancel in one simulation tick.
- Added a bounded second stage for legacy jobs which ignore normal cancellation. After another unchanged-job grace
  period, only the loaded/unbound recovery path force-clears that already-cancelling sequence and stops stale
  navigation; normal, managed and newly produced flights are ineligible for this fallback.
- Recovery no longer depends on `DynamicGroundEntityMb`: saved vehicles which failed renderer initialization are
  handled by the same synchronized path and report `unpaused serialized saved heli` / `reset stalled saved stock job`
  diagnostics in the game log.

## 0.13.2 (SAVED-ORPHAN-TAIL-RECOVERY)

- Fixed the saved mass-cancellation state where `HasTrueJob=false` and `CurrentJob=null`, but an internal
  `VehicleJobsSequence` tail remains while navigation is already stopped. These helicopters are now rebound to one
  stable home before the pending-sequence guard and the orphaned tail is cleared in the synchronized depot tick.
- Added a slower fallback for a stopped non-true current tail, while preserving genuine navigation, refuelling and
  normal between-delivery transitions. Per-vehicle staggering spreads recovery of hundreds of aircraft over 32 sync
  ticks instead of executing every job reset in one frame.
- Added recovery diagnostics (`cleared orphaned ... job tail`) so a loaded save can be verified directly from the
  game log instead of inferring its state from the visual hover.

## 0.13.1 (CANCELLED-JOB-RECOVERY)

- A job cancelled before the C-place handoff no longer makes the helicopter collect an attachment only to return it
  immediately. The active flight is redirected to its stand while the personal attachment stays safely in storage.
- Added an exact-job, no-progress watchdog for stock cargo routes orphaned by deleting sources, receivers or a large
  batch of work. The reset is requested from the render driver but revalidated and executed only inside the safe
  simulation sync section; managed landing, pickup, hidden and taxi phases are explicitly excluded.
- Cleared the T1 production climb-out by removing the decorative overhead portal that helicopters passed through.
  Moved the service tanks to the deck edge and shifted `EQUIPMENT C01-C05` so the label remains unobstructed.

## 0.13.0 (HELIPORT-VISUAL-OVERHAUL)

- Rebuilt Open Heliport T1 as a compact two-spine, twenty-stand counterpart to T2: filled angled bays, curved gold
  lead-ins, H pads, turn heads, blue guide lights, deck-panel seams, protected equipment cradles and assembly cells
  now share the same industrial visual language as the large base.
- Replaced T2's blocky orange crane with a closed 91-part rail-mounted lattice crane including feet, slew ring,
  braced tower, sealed cab, twin truss boom, counterweight, trolley, cable, hook, ladder, work light and guard rails.
  T2 service-yard drains and equipment stops were added without restoring fake decorative cargo.
- Added explicit `OH_StDir01..40` world-space direction markers to both tiers. Parked helicopters now align with the
  painted angled bays and face their taxi spine independently of Blender Empty/mesh FBX axis conversion.
- Added separate transparent 512 px construction-menu icons for T1 and T2, with the large base visibly showing its
  four-pad layout, control tower and crane.
- Recalculated opaque T2 detail normals and forced solid material alpha while preserving the dedicated Unity repair
  path for the legacy one-sided deck and marking meshes.

## 0.12.3 (LOCAL-DEPOT-FLEET-CAP)

- Replaced the global fleet ceiling with a strict per-entity ceiling. A T2 queue now counts only aircraft whose
  home is that exact T2 (maximum 40); constructing an empty T1 no longer unlocks queued production at a full T2.
- Production fails closed until the producing prefab's stand markers are resolved, preventing an ownerless vehicle
  from being finalized during the short building/render initialization window.
- A scrap/replacement button no longer releases capacity immediately. The local slot remains occupied until
  `VehiclesManager.DestroyVehicle` actually removes the old helicopter, so the queue cannot replace it mid-flight.
- Added save recovery for aircraft already stranded at an assembly line by 0.12.2: their physical production lane
  restores the original depot before any T1/T2 dispatcher can claim them. They remain attached to that base and can
  be scrapped normally or take a newly freed local stand; they are never silently transferred to a different tier.
- In-flight helicopters whose runtime home is not yet reconstructed after loading reserve conservative capacity
  until they return. This can briefly hold a queue after load but prevents invisible overbooking.
- Moved cross-thread home/capacity registries to concurrent collections so simulation-thread production checks and
  synchronized Unity dispatch cannot race while a newly manufactured vehicle is being bound.

## 0.12.2 (UNITY-BUNDLE-COMPATIBILITY-HOTFIX)

- Rebuilt the T2 `open_heliport` bundle with the game's exact Unity version (`6000.0.66f1`). The previous package
  was accidentally serialized by Unity `6000.5.2f1`, which Captain of Industry correctly rejected and replaced
  with its green/white missing-prefab checkerboard.
- Kept the T2 visual cleanup from 0.12.1: the broken decorative container-plane cluster is absent while all 40
  stands, 10 functional C-yard markers, four production pads and 313 valid renderers remain.
- Added hard build-time guards and a serialized-bundle load test so a mismatched Unity editor cannot publish the
  T2 model again. Release packaging now validates the Unity version of every shipped asset bundle.

## 0.12.1 (STRICT-HOME-AND-DEMOLITION-MIGRATION)

- Fixed cross-tier ownership at its source. `IVehiclesManager.Trucks` is global, but a helicopter now receives its
  immutable home-depot id at production time and every foreign T1/T2 prefix rejects it before in-place adoption or
  idle debounce. A full home waits for its own capacity instead of silently falling through to a neighbouring base.
- Added an immediate `VehicleDepotBase.OnDestroy` teardown path. Demolishing a heliport clears its landing targets,
  C-yard visuals, stand/column leases and active motion in the same simulation teardown instead of leaving aircraft
  and containers suspended until the old twenty-second renderer watchdog expired.
- Added capacity-aware home migration. Managed aircraft fly continuously to another live heliport; aircraft whose
  jobs are still active retain a pending rehome flag and return after work. Overflow safely holds without equipment
  until a new T1/T2 provides both fleet and physical C-yard capacity.
- Removed the broken six-material `OH_ContainersDeco` plane cluster from the actual T2 prefab. A separate cleaned
  Blender source copy also removes that mesh, legacy actions, authoring cameras and lights without overwriting the
  user's open working file. Functional C01-C10 positions remain runtime-only.

## 0.12.0 (TIER-ONE-PIPELINED-HELIPORT)

- Added a separate compact Open Heliport T1: a clean Blender model with twenty full-size parking stands, two taxi
  spines, five four-layer equipment columns, two production lines, two service pads and authored FIFO holding points.
  The original 40-stand building keeps its `OpenHeliport` id and becomes T2, preserving existing saves.
- Made stand capacity, equipment-column count and production-line count marker-driven per depot. T1 (20/5/2) and
  T2 (40/10/4) can operate together without sharing global yard dimensions or production limits.
- Added a per-column FIFO arrival pipeline. A returning helicopter keeps its stand/equipment lease while its C-column
  is busy and flies to a holding point instead of cancelling the reservation and repeating the 72-tick idle debounce.
- Arrival ownership now releases a C-column when the aircraft has physically cleared a 14 m rotor-safe radius, not
  after the complete stand landing. The next wave can approach while the previous wave continues to parking.
- Removed all 825 imported animation/visibility clips, cameras and lights from the legacy T2 bundle. Mechanical arms
  remain driven explicitly by production state; decorative renderers no longer inherit unrelated FBX visibility curves.
- Rebuilt the T1 render hierarchy with nine supported materials, fifty-three renderers, solid raised markings and no
  fake containers on functional storage cells. Runtime attachments are visually and structurally separate from decor.

## 0.11.0 (HELIPORT-KINEMATIC-MOTION)

- Added one allocation-free kinematic motion core for every scripted heliport animation. High game-speed frames are
  integrated in fixed 1/60-game-second slices, while authored legs use quintic trajectories with continuous position,
  velocity and acceleration instead of independent `SmoothStep` starts and stops.
- Rebuilt production departure: open assembly lines now spool rotors on the deck and perform one smooth accelerated
  climb to departure clearance. The old nearly-zero elevator, top pause and second constant-speed vertical jump are gone.
- Reworked the four production arm pairs into explicit reach, assembly-pass and release strokes. They follow game time,
  move only on the lane which is actually constructing, and retract before the completed helicopter lifts clear.
- Rebuilt C-place and stand movement around velocity-preserving segments. A redirected first flight keeps its current
  velocity, final approach blends progressively into the authored stand heading, and descent eases to zero at touchdown
  before the dispatcher receives its grounded acknowledgement.
- Added acceleration-limited rotor, altitude and winch motion. The cable slows near the attachment, waits for a stable
  contact before handoff, and reels in without the old constant-speed stop. Sling yaw and pendulum physics use the same
  fixed substeps and remain stable at x15.
- Replaced the 42 m/s departure snap with an acceleration-limited moving-target intercept. Visual control is released
  only after both position and velocity have converged on the live job route.
- Kept the 0.10.10 multi-heliport dispatcher, home-depot ownership, stand reservations and ten independent C-columns
  unchanged; the rewrite is confined to visual/kinematic ownership and its completion handshakes.

## 0.10.10 (MULTIBASE-ROUTING)

- Fixed multi-heliport ownership. The vehicle manager is global, so every heliport previously advanced the same idle
  helicopter and whichever depot updated first claimed it. Aircraft now retain a stable home depot while working;
  only a missing/demolished/full home falls back to the nearest compatible heliport.
- Removed hidden long-distance ground routes during storage return. The visual helicopter now owns the complete
  container-to-stand flight; at its already-visible touchdown the hidden simulation entity is committed once to the
  reserved pathable stand, then delayed render samples are flushed before handoff.
- Fixed the `Value of Fix64 ... expected < 576` assertion storm seen when 80-120 helicopters received work together.
  Helicopter logistics now use a one-tile clearance graph (visual size is unchanged), keeping converted waypoints
  inside the engine's fixed 24-tile terrain segment bound.
- Idle debounce now advances once per helicopter instead of once per live heliport per sync. Three or four bases no
  longer shorten the scheduler grace period or redistribute an entire returning fleet to the first platform.
- State-timeout diagnostics now include depot, stand and cargo-slot identifiers for direct multi-base tracing.

## 0.10.9 (CANCELLED-DEPARTURE-RECOVERY)

- Fixed a cancelled first job leaving a helicopter in `PickupDeparting` until the generic 1200-tick timeout. The
  reproduced heli 83 case held C06 for roughly two minutes and serialized later storage returns behind that lease.
- A physical C-column is now released as soon as the attachment is reeled in and the aircraft has cleared the slot,
  before the visual helicopter merges into the distant live route. A stalled route can no longer block its column.
- If the stock job disappears after pickup, the aircraft immediately stops departure, obtains a fresh storage lease,
  returns the live attachment through the normal yard path, and lands. The cancelled job never waits on route motion.
- If a still-valid route fails to leave its SIM origin, a bounded 180-tick recovery flies the visual pose back to the
  live SIM pose and hands control over there instead of pinning the aircraft over the yard for 1200 ticks.
- Player scrap or replacement requests now outrank the heliport state machine. All stand, yard and pickup leases are
  torn down on the next sync and the stock vehicle lifecycle receives the helicopter immediately.

## 0.10.8 (JOB-CHAIN-HANDOFF)

- Fixed the scheduler boundary that treated `HasTrueJob=false` as a fully idle helicopter even while the stock
  `VehicleJobsSequence` still contained cleanup/navigation work. Heliport return now starts only after the complete
  job sequence is empty and navigation has stopped.
- Increased the idle confirmation from 12 to 72 simulation ticks. Captain of Industry balances waiting trucks only
  once every 17-23 steps, so the old threshold recalled helicopters before their first guaranteed scheduling pass;
  the new threshold covers three worst-case balancing passes.
- A new cargo job assigned while an attachment is being returned now chains at the C-place. Once the attachment is
  safely committed, the existing visual flight is redirected to the required attachment and live route; the stand
  is freed and the intermediate land-then-immediate-takeoff cycle is skipped.
- A job arriving during the C-place-to-stand approach redirects the active pose owner continuously, including after
  visual touchdown but before the simulation handoff, so no reset or teleport frame is exposed.

## 0.10.7 (DIRECT-FLIGHT-POOL)

- Replaced the visible factory-to-stand movement with the helicopter flight driver. The authoritative ground entity
  still completes its route in the background for a safe simulation handoff, but the aircraft now flies and lands
  smoothly instead of hovering for roughly 40 seconds or stepping between sparse positions at high game speed.
- A job assigned during that first flight redirects the existing visual trajectory directly to the selected C-place;
  it never exposes a ground-route frame and never makes an intermediate stand landing.
- Converted stored attachments into an interchangeable ten-column pool. If a helicopter's personal attachment is
  buried in a busy column, it atomically borrows the stable top attachment from another free C01-C10 column and gives
  its buried lease to that parked owner. Up to ten physical pickup columns can now dispatch concurrently.
- Column ownership and pickup reservations remain authoritative, so two helicopters can never lower onto the same
  C-place and concurrency cannot exceed the ten authored storage positions.

## 0.10.6 (CLEAN-FIRST-FLIGHT)

- Newly built helicopters now emerge from the four assembly lines without a box. Their reusable attachment is
  allocated and materialized directly at a free C01-C10 storage lease as factory output; the aircraft never flies
  an empty attachment to the yard merely to put it down.
- Added a production ticket which survives the vertical emerge and is consumed only after the first-flight state
  machine is installed. With no job the route is assembly line -> reserved stand. With an immediate job it is
  assembly line -> correct stored attachment -> live pickup route, with no intermediate stand visit.
- The first-job departure merge now uses the aircraft's actual pickup-start origin (assembly line or stand) rather
  than assuming every job begins on a stand. A job assigned during the direct parking leg cancels that route and
  diverts to the storage attachment before touchdown.
- Attachment kind is resolved from the real enqueued pickup and the staged yard visual is atomically replaced with
  a container, tank or log carrier before departure. A cancelled first job returns to the direct-stand branch.
- Ordinary post-work return remains one continuous sequence: work -> C01-C10 drop -> reserved stand -> one landing.

## 0.10.5 (STABLE-STANDS)

- Removed the second `DrivingEntity.SetTurningTarget` manoeuvre from the final parking handshake. It could race the
  vehicle job state with `IsDriving=true` after navigation had ended, making already parked helicopters rotate in
  place forever. A stale 0.10.4 turn is now cancelled once; after all movement stops, only the authoritative yaw is
  committed and the delayed render buffer is allowed to settle. Position and jobs are untouched.
- Replaced the always-on blend of up to 128 imported Legacy clips with a deterministic controller for the eight
  authored production arms. Only the lane currently building or releasing a helicopter moves; inactive arms ease
  back to their exact authored rest transforms and all movement freezes while the game is paused.
- Published construction and active-line state through thread-safe registries so Unity animation never enumerates
  a simulation-owned mutable collection.

## 0.10.4 (NO-TELEPORT-PARKING)

- Replaced the container-to-stand handoff with a no-teleport pipeline. The authoritative Truck routes to its
  reserved stand in the background, turns through the normal driving API, and remains stable for eight sync ticks
  before the render owner is released. No parking path writes the Truck position or rewrites the MB pose on exit.
- Removed the saved-stand position teleport as well. Existing parked helicopters now normalise their heading by a
  normal in-place turn and the same delayed-buffer settling handshake.
- The pickup path remains direct: after the attachment is reeled in, the real job starts immediately and the visual
  helicopter merges from the container place into that live route without revisiting its stand.
- Added four independent finished-aircraft production lanes using the authored `OH_Arm_L1..L4` locations. Up to
  four completed helicopters can spool and climb out concurrently; a fifth waits without consuming another lane.
  Spawn coordinates are supplied before `DrivingEntity.Spawn`, not teleported after a render frame.

## 0.10.2 (OPEN-HELIPORT-REWRITE)

- Production-arm animations on the Open Heliport. The model's 8 robotic arms now loop their authored motion. The
  Legacy animation clips are wired onto the model node (not the wrapper root) so the clip paths resolve to the real
  bones, and the mod plays them via a small isolated MonoBehaviour (HeliportArmsMb), capped for safety. The building
  loads and parks helicopters regardless of the animation - it is decorative and cannot affect flight/parking.
- The bundle is coloured from the GLB's own 46 materials (charcoal deck, gold/white markings, orange arms, concrete).

## 0.10.0 (OPEN-HELIPORT-REWRITE)

- COMPLETE REWRITE of the heliport. The old single-pad hangar building and its 4559-line dispatcher, door/lift
  animation and cargo-yard were deleted and replaced by the new large **Open Heliport**: 4 landing pads, 40 parking
  stands, 10 container spots. Helicopters now fly home and PARK on a stand, visible, until they get a job - no hangar,
  no hiding, no second move. The whole deck is a ground-routable surface, so a helicopter simply flies to its stand
  and descends once; the game owns its horizontal position throughout, which structurally removes the spurious
  extra take-off / teleport that the old design kept producing.
- Fleet cap: a heliport will not build more helicopters than it has free parking stands (summed across all heliports).
- The building is coloured from the model's own materials (textures + arm animations are a later pass).

## 0.9.6 (SLING-YARD-NAV-HARDFIX)

- Fixed the side-mounted sling during heliport storage. Imported C-01..04 marker transforms contained a hidden
  GLB axis-conversion pitch/roll; cargo placement now preserves only the painted bay's horizontal long-axis
  heading and forces world-up for both the live sling and its persistent yard copy. The four straps therefore
  meet above the container roof instead of lying along a side or bottom edge.
- Replaced the moving 10 cm navigation target and 15 cm capture gate introduced in 0.9.5. Final cargo approaches
  now use one stable root target from an atomic same-frame payload/root offset, a practical capture radius, and an
  exact local payload-to-marker correction during winch operation. This removes repeated turns and GetUnstuck
  loops while keeping the final container precisely centred on C-01..04, even during render lag.
- Applied the same stable approach and exact centring to departure pickup, preventing helicopters from circling
  their own parked attachment before a job.
- Activated the authored `ContainerDropStaging` route in both directions. The ground-vehicle navigator now reaches
  the safe exterior waypoint first and performs one short final leg, instead of attempting a direct route through
  the heliport footprint or over its roof.
- Activated round-robin yard allocation with a short failed-route cooldown. A pending visual removal must finish
  before its lease can be reused, so recovery no longer resurrects the same failed slot and blockades every
  helicopter behind the depot's yard owner.

## 0.9.5 (PAYLOAD-CENTER-HARDFIX)

- Fixed the actual cause of side-mounted cargo-yard drops: navigation had centred the helicopter entity root on
  C-01..04 even though the visible S-64 belly hoist is 1.52 m behind that root. The renderer now publishes the
  live payload origin and storage converts every authored payload target into the required vehicle-root target.
- Arrival and pickup complete only after the payload itself, rather than the helicopter pivot, remains within
  15-20 cm of the painted slot centre for three consecutive updates.
- Authored slot rotation is applied during the final approach, before descent begins, eliminating the last
  rotation-dependent offset and the crooked-to-straight handoff frame.
- Static yard copies are seated and centred from their combined renderer bounds on all three axes. Containers,
  tanks and log cradles therefore land exactly at the marker even when their prefab pivots differ.

## 0.9.4 (PAUSE-JOB-STORAGE-HARDFIX)

- Pinned the complete last rendered helicopter position and rotation while the game is paused. Stock ground-vehicle
  interpolation can no longer display the terrain-level simulation pose and then jump back on resume.
- Cancelled empty orphan job sequences immediately. If a receiver becomes unavailable and the game withdraws the
  current delivery, the helicopter stops navigating to the obsolete destination and requests new work next tick.
- Disabled terrain auto-parking whenever at least one live Heliport Complex exists. If all 20-slot hangars are full
  or their approach resources are busy, unassigned helicopters remain airborne instead of parking beside a wall.
- Removed the navigation watchdog from the winch-only pickup-retraction phase and extended its bounded timeout,
  preventing a healthy sling animation from being misclassified as a stuck flight route.
- Tightened visual capture at cargo drop and pickup. The sling reaches the exact authored slot centre before handoff,
  so the cable remains above the attachment and the grounded model no longer performs a second sideways snap.
- Grounded attachment visuals now clone the exact returning helicopter's materials. Container colour is preserved
  across the slung-to-static handoff instead of falling back to a cached yellow template.
- Rebuilt the Heliport Complex with 13.8 m assembly openings and 12.765 m lifts for the measured 13.602 m S-64 body,
  moved both assembly spawn markers to the enlarged shafts, and removed overlapping rear equipment geometry.
- Exported `Heli_Complex_0.9.4.glb` as the corrected editable source and rebuilt the Unity AssetBundle.

## 0.9.3 (GEOMETRY-WALLS-GROUNDOPS)

- Rebuilt the supplied heliport model with a double-sided baked shader. Thin authored walls now render from both
  directions instead of disappearing in game while remaining visible in Blender.
- Corrected the two assembly hatches: each opening is now 9.44 m, opposing leaves meet exactly at the centre, and
  neighbouring covers retain a 0.32 m structural gap instead of intersecting.
- Main rotor blades stay hidden while a newly built S-64 is below the assembly roof and appear only after the hub
  clears the hatch, preventing full-diameter rotors from clipping through the two adjacent shafts.
- Resized all four authored cargo-yard outlines to 6.50 x 2.25 m from the measured runtime attachment footprint
  (container 6.118 x 1.743 m; tank 6.120 x 1.987 m).
- Smoothed the last portion of storage touchdown, added a short skid-settle beat before taxi, and kept both rotors
  at 22% ground idle while a visible helicopter rolls from the pad into the hangar.
- Exported `Heli_Complex_0.9.3.glb` as a corrected, editable Blender source with intact movable pivots.

## 0.9.2 (MATERIAL-DUALBAY-ROUTE-STABILITY)

- Re-baked the supplied `Heli_Complex.glb` with a darker concrete/paint atlas that preserves the authored grey,
  graphite, orange, yellow and emissive details under the game's bright outdoor lighting.
- Enlarged both assembly hatch frames and doors from 8.0 m to 10.56 m and both lift decks to 9.77 m without
  changing their authored centres, runtime markers or animation pivots.
- Made authoritative helicopter altitude the final per-frame visual pose, preventing pause/resume ground jumps.
- Removed cargo helicopters from all vanilla Vehicle Depot / Auto Vehicle Factory build lists.
- Removed the shared bay cooldown and reduced the depot spawn interval; the two assembly shafts can now host two
  simultaneous emerge sequences, while the existing two-aircraft safety gate still rejects a third.
- Added job-state debounce and a post-abort recruitment cooldown. Transient scheduler gaps no longer repeatedly
  reassign one helicopter among multiple heliports, eliminating the observed circling/stalling over forests.

## 0.9.1 (PAUSE-MATERIAL-HATCH-HOTFIX)

- Fixed ordinary flying helicopters being rendered at terrain height while the game was paused.
- Rebalanced the new heliport material for the game's bright lighting so its baked colours remain visible.
- Corrected all four assembly roof hatch directions to match the authored GLB animation.

## 0.8.5 (FSM-YARD-ATOMIC)

- Reworked idle-storage traffic into explicit per-depot resource queues: cargo-yard work, exterior approach, landing pad and door transit no longer serialize unrelated heliports or block the next attachment drop while the previous helicopter is landing.
- Removed the render-only departure/landing teleport path. Final descent now eases from the actual visible pose in 0.65-2.5 seconds and finishes at the authoritative simulation X/Z, preventing the landing jump when normal simulation resumes.
- Added an off-screen landing handoff: pending plans are consumed by the wall-frame bootstrap through a cached live entity, so a helicopter no longer hovers indefinitely waiting for a visibility-dependent render callback.
- Added navigation watchdogs, bounded retry/recovery and safe exterior route legs above measured building bounds. Helicopters climb vertically clear of the roof before crossing the complex and release their real job only outside its footprint.
- Made yard ownership atomic: one helicopter owns exactly one attachment lease across all depots, stale park/unpark requests cannot delete a reused model, and a departing helicopter reserves the complete stack column before pickup.
- Departing helicopters can safely exchange a buried logical lease with the uppermost idle attachment, preserving physical stack order while guaranteeing that each aircraft retrieves an accessible personal container, tank or log cradle.
- Attachment drop and pickup are now atomic visual handoffs. The slung payload remains visible until the persistent grounded model exists; on pickup the ground model remains visible until destruction is acknowledged, eliminating missing, doubled and flickering cargo.
- Planned pickup kind is published before rendering and retained through empty transfers. Liquid jobs show a tank immediately instead of flashing a container, including after unloading and while preparing the next liquid route.
- Corrected the authored `CY_Deck` contact surface and added a small seating inset, so the bottom container/tank rests on the storage platform and upper layers rest directly on the model below.
- Fixed pause handling for the storage FSM, door animation, emerge animation and watchdog. Pausing no longer advances timers or moves aircraft, and resuming no longer snaps them to a different pose.
- Fixed partial/late model transform discovery and selection collider restoration, preventing permanently bare rigs and helicopters that cannot be clicked after leaving virtual storage.
- Old saves or never-rendered helicopters with no published attachment kind now use a recoverable personal-container fallback instead of waiting in the air forever; the correct planned kind is still swapped in before departure.
- Reset now clears cached job decisions, queue scratch state, yard leases, pickup reservations and runtime landing handles, preventing recycled vehicle IDs from inheriting stale state after loading another game.
- Harmony patches for hot vehicle render methods remain deferred until scene initialization is fully complete, avoiding patch/JIT work during delayed deserialization while keeping the essential Animator-less heliport guard installed early.

## 0.8.4 (PIPELINED-ARRIVALS)

- Arrival queues now pre-climb at their separated holding points while another aircraft uses the cargo yard, eliminating the serialized climb delay before every attachment drop.
- The cargo-yard lock is released immediately after the persistent attachment model is confirmed visible. The next helicopter may start lowering while the previous one waits for or approaches the landing pad.
- Removed the redundant hidden-sling retraction wait; pad/door ownership remains exclusive until the previous helicopter is physically inside the hangar.

## 0.8.3 (LATE-PORT-REBALANCE)

- Fixed loaded games assigning every idle helicopter to the first heliport renderer that registered. Aircraft still travelling to or waiting in holding are now re-scored whenever another already-built heliport appears.
- Multiple heliport complexes now run their independent cargo-drop and landing pipelines concurrently; aircraft that have already begun lowering or landing are never redirected mid-animation.
- Static and winched attachments now use a 2 cm contact inset so container and tank bottoms visually meet the authored storage deck with no daylight seam.

## 0.8.2 (PARALLEL-YARD-PAD-QUEUE)

- Split the cargo-yard and landing-pad locks: one helicopter can lower its attachment while the previous aircraft lands and enters the hangar, substantially increasing arrival throughput without skipping animations.
- Rebuilt the holding fan around the S-64 rotor diameter, increased cross-depot separation to 30 m and tightened arrival capture so queued helicopters no longer overlap in a cluster.
- Parked containers and tanks now sit 1 cm above the authored deck or lower attachment instead of visibly floating by 5 cm.
- Made every normal, abort and timeout release destroy the helicopter's owned static yard model before restoring its sling, preventing bare helicopters and duplicated/orphaned attachments.
- Added cleanup for stale yard/drop reservations and a bounded visual-handoff failure path so a missing cosmetic mesh cannot block the whole port forever.

## 0.8.1

- Added complete Sikorsky S-64 Skycrane model attribution and CC BY 4.0 licensing information.
- Release version incremented for distribution; gameplay code is unchanged from 0.8.0.

## 0.8.0 (ATC-AUTHORED-YARD-ROUTING)

- Replaced the heliport AssetBundle with the supplied `Heliport_ATC_Complex` model, optimized to nine renderers while preserving the named roll door, two lifts and four hatch panels used by runtime animation.
- Added an authored `ContainerYardAnchor` at the centre/top of `CY_Deck`; parked containers and tanks now use the elevated platform surface instead of terrain underneath the deck.
- Recalibrated the 1:1 model: main pad and garage floor at 6.05 m, assembly lifts at 6.45 m, 7.1 m lift travel, mirrored Unity garage interior, tower bays, footprint and occupancy height.
- Replaced the direct yard-to-pad route with `yard -> exterior gate -> pad overhead -> vertical descent`. The game navigator can no longer draw a horizontal line through the hangar footprint.
- Height gates now read the actual interpolated Transform pose, not FuturePosition. Visible Y converges smoothly on wall time, preventing the render model from lagging below a roof-safe simulation pose.
- Removed the SIM-tick winch timeout that let an upper-layer attachment enter the garage. Landing cannot begin until physical grounding and successful static-visual handoff are both confirmed.
- Storage drops lock the sling to the platform's world rotation from the start of the descent, eliminating the crooked-lowering then instant-straight snap.
- The authored platform uses a centred 2x2 footprint with five strictly owned layers for twenty helicopter attachments.

## 0.7.10 (VERTICAL-OVERHEAD-PAD-APPROACH)

- Returning helicopters now use hard three-phase arrival legs: lock X/Z and climb above the measured depot roof, fly horizontally at that safe altitude, then descend vertically only after capturing the exact helipad centre.
- The incoming holding-to-container-yard leg uses the same vertical clearance barrier, so a helicopter carrying its attachment cannot cut diagonally through the hangar while approaching the drop point.
- Every horizontal arrival leg retains the roof-safe minimum altitude until its destination is reached. Landing clearance is still withheld until pad-centre capture.
- Added transition logging with actual and required world altitude, making it possible to verify that roof clearance was genuinely reached before navigation resumed.

## 0.7.9 (SINGLE-PAYLOAD-SAFE-APPROACH)

- Removed the unreliable ProductsRenderer/cosmetic handoff entirely. Every helicopter now uses one persistent sling attachment mesh for empty, loading, loaded, unloading and flight states, so a queued GPU render can never make the container disappear.
- Product type and icon still update from the authoritative cargo/planned pickup; only the unstable duplicate renderer was removed.
- Returning helicopters keep a hard roof-clearance altitude from the yard all the way to the exact pad centre. Landing clearance is granted only after centre capture, preventing the yard-to-pad route from descending through the heliport building.
- Reasserts whole-sling hiding after the static yard handoff so the intentional empty return-to-hangar phase has no confusing dangling spider/cables.

## 0.7.8 (TRANSFER-VISIBLE-ROOF-CLEARANCE)

- A lowered sling now always uses one stable cosmetic container/tank during loading and unloading. GPU unit-cargo rendering is enabled again only after the sling is reeled up, so the container cannot disappear at zero/partial transfer quantity.
- Hangar rotor spin-up and vertical climb use fixed sync steps instead of near-zero DeltaSimStepsApprox; takeoff no longer spends ~11 sim seconds in state 3.
- Storage departure receives a hard minimum altitude above the measured heliport roof before any horizontal yard/job leg is allowed.
- Normal airborne altitude correction uses a stable minimum sync interval, allowing LineMax roof detection to raise the helicopter before its horizontal simulation path reaches factories.
- The fixed TakeoffStartY target now also incorporates the depot roof-clearance altitude.

## 0.7.7 (ATOMIC-PAYLOAD-TAKEOFF)

- Fixed missing attachments: an unknown/empty FlatBed snapshot is no longer treated as a real GPU-rendered product, and stale render-success state can no longer suppress the cosmetic fallback.
- Restored twenty strictly owned visual attachment places per depot in a non-overlapping compact 3x2 yard with up to four correctly seated layers.
- Yard pickup is now atomic: the rig and cable descend while the static ground payload remains the only visible payload; after the main thread confirms that ground GameObject is destroyed, exactly one slung payload appears and reels in.
- Fixed the moving takeoff threshold by capturing a fixed TakeoffStartY. Storage departures climb straight up rapidly to StartY+12 instead of waiting 9--11 sim seconds for an unreachable moving target.
- Before releasing JobHeld, the dispatcher clears its terminal yard DrivingTarget, allowing the untouched real job to issue its source/destination immediately instead of circling or becoming idle again.
- Aborted storage sequences restore the slung payload whenever their owned static yard model is removed.

## 0.7.6 (OWNED-YARD-JOB-HANDOFF)

- Final hangar landing now eases for 0.9 seconds from the actually visible helicopter pose and writes altitude only; it no longer snaps from an already-advanced FuturePosition.
- While a stored helicopter exits and retrieves its attachment, only its real job advancement is held. Game movement continues normally, eliminating the fight between the yard target and the job target that caused circles and U-turns. The untouched job resumes immediately after reel-in.
- Yard slots are atomically reserved and carry an explicit owner ID: one helicopter has exactly one container/tank slot, and a stale helicopter cannot park in or remove another helicopter's slot.
- The owned yard model is changed to the known next pickup kind before departure, so liquid jobs use that helicopter's tank instead of briefly showing a container.
- Fixed `ReRenderAtSling` overload resolution (`Proto`) which previously logged `AmbiguousMatchException` and disabled genuine product re-rendering for the session.

## 0.7.5 (CLEAN-HANGAR-SEQUENCE)

- Parked attachment visuals now use terrain height only and are rejected if the complete yard footprint is not open ground; they can no longer inherit a roof/occupancy height and float above the heliport.
- Replaced the overlapping 20-object cosmetic grid with a correctly spaced 2x2 two-layer yard. The hangar still stores 20 helicopters logically.
- Storage landing is now a fast, continuous vertical descent after precise game-navigation alignment. Removed the landing `TeleportTo` snap.
- Replaced the scripted U-turn/corridor departure with: taxi to pad centre, normal vertical takeoff at full rotor RPM, direct game-navigation leg to the yard, grounded winch pickup, reel-in, then release to the job.
- Scripted poses are reasserted while paused, eliminating the pause/unpause visual position jump.
- Landed rotors stop fully; lift starts only at full rotor speed. Travel attachments reel tightly to the helicopter.

## 0.7.4 (LOADGATE-FIX)

- Fixed a Linux/Steam-Proton crash where the game could fail to load with the mod enabled (peach-coloured background, resetting loading bar, half-built HUD, then a crash). The mod's Harmony patches were being applied on a fixed ~1-second timer that, on slower machines loading larger saves, still fired while the game was still initializing the scene (its "delayed deserialization" stage plus a tail of forced warm-up frames) — interrupting the load. The one-time patching now waits until the scene is fully initialized and the loading screen is gone (IMain.IsInitializingScene turns false) before applying, so it can no longer land mid-load however long the load takes.

## 0.7.3 (LANDING-YARD-HARDFIX)

- Helicopter build time cut ~4x: Tier I/II/III now take 30/45/60 seconds (only these three helicopters; other vehicles unchanged).
- The build-complete emerge (hatch, elevator ride, lift-off) is now ~2.5 seconds instead of ~7, with the hatch opening faster in step so the helicopter never rides up through it.

## 0.7.2 (LANDING-YARD-HARDFIX)

- The cargo yard now anchors to an authored ContainerYardAnchor transform if the heliport model provides one (and logs its chosen world position either way), so the drop zone can be placed precisely instead of guessed near the pad edge.

## 0.7.1 (LANDING-YARD-HARDFIX)

- Rewrote the final touchdown as a deterministic, frame-driven descent (the helicopter briefly pauses, is set straight down onto the pad centre in ~2 seconds, and is committed to the simulation there) instead of relying on the physics/render path to settle a landing that could hang for tens of seconds.

## 0.7.0 (LANDING-YARD-HARDFIX)

- After a helicopter sets its load down in the yard, the sling — spider, straps, cable and hook — now fully disappears, so it no longer flies to the pad with an empty rig dangling underneath.
- Stored yard attachments now rest with their bottom face on the ground surface instead of floating, and a stacked second layer sits on the actual top of the object below it.
- A full yard no longer overwrites another helicopter's parked model; when full, the helicopter simply stores without a cosmetic drop.
- Storage commands are no longer silently dropped if issued a moment before the helicopter's first render.
- Startup now logs a unique build banner so a game log unambiguously shows which build actually loaded.

## 0.6.8

- Fixed a yard slot and its model leaking (and a possible duplicate attachment) when a helicopter was pulled back to work or force-released after it had already requested a yard drop.
- Hardened the departure simulation-commit against a game method being overloaded in a future update.

## 0.6.7

- Fixed the wrong attachment (container) flashing for a frame when a helicopter reappears from storage or switches load type: attachment visibility is now resolved in one place and never blank-enables all attachment meshes at once.
- A helicopter now sets its attachment in the yard only once the permanent yard model actually exists (no blink/gap), and only after the load has truly reached the ground (never a model left floating in the air).
- Departing helicopters now commit their scripted takeoff to the simulation once at the exit waypoint, so the real job continues from outside the complex instead of snapping back through the building.
- The vertical-takeoff height now uses the building's measured height instead of a fixed guess.

## 0.6.6

- Fixed parked yard attachments (tanks/containers) flickering: they are now permanent objects in the scene instead of being redrawn each frame from an unreliable render hook, so they stay visible in every frame.

## 0.6.5

- Fixed helicopters hanging for tens of seconds directly over the pad centre before touching down: touchdown is now confirmed from the simulation position (the helicopter stops driving when centred) instead of a render-interpolation speed that never settled at zero distance.
- Fixed a helicopter's job-state check throwing inside the game and briefly flipping the helicopter between busy and idle, which disrupted storage.

## 0.6.4

- Helicopters now leave the hangar safely: they taxi to the pad centre, take off STRAIGHT UP above the roof, then head out — the real job can no longer drag a departing helicopter straight through the hangar.
- A departing helicopter reclaims its own attachment from the yard (drops to it, picks it up, lifts back up) before flying out to a waypoint clear of the complex, and only then is handed to its job.
- Hardened patch loading: if a future game update renames a hooked method, only that one hook is skipped instead of disabling the whole mod.
- Fixed a yard slot that could stay marked "occupied" if a drop was abandoned.

## 0.6.3

- The hangar cargo yard now shows the actual helicopter ATTACHMENT (container, tank or logs) that a returning helicopter sets down — tanks and logs finally appear, where the old product-render could only show container-type cargo.
- A returning helicopter always leaves its attachment in the yard and stores/leaves empty, so the same attachment is never shown both on the helicopter and in the yard.
- Attachment visuals never touch real cargo; the game's Truck.Cargo is unchanged.

## 0.6.2

- Idle helicopters are now distributed across ALL heliports by a distance + load score (nearest, least-loaded, free pad), instead of every idle helicopter piling onto whichever port happened to tick first.
- Holding points now form a validated fan on the open sides of the complex (never through the hangar or tower), each checked against the building occupancy map and kept apart from other ports' holding points.
- A helicopter now flies to its holding point and only then joins the landing queue, so a distant helicopter no longer reserves the pad before it has arrived.
- Recall timing no longer speeds up as you build more heliports.

## 0.6.1

- Fixed slow storage landing: a recalled helicopter now descends to a firm deck target and touches down only when centred over the pad (not hanging above it). Storage descent uses a faster, storage-only speed.
- The removed the blind per-approach re-issue of the landing command that was restarting the descent; added a one-shot stuck-recovery with a diagnostic log and approach-time logging.
- Fixed the next-load silhouette: a helicopter heading to a liquid pickup now shows the tank (not a container). The next attachment type is applied as soon as the pickup job is assigned; an unknown next type keeps the current attachment instead of defaulting to a container.
- Made the pickup-job hook type-safe (no reflection / no swallowed errors).

## 0.6.0

- Heliport now brings idle helicopters home into a real airborne holding queue (distinct ring points) instead of stacking them.
- A single pad/door is served one helicopter at a time (FIFO), with stored helicopters that get a job taking priority to leave.
- Faster storage landings: the authorised pad helicopter descends during its final approach.
- Helicopters carrying a container set it down in the hangar's cosmetic cargo yard (20-slot 5x2x2 grid) before parking; the real cargo is never altered.
- Stored helicopters are hidden (virtual storage of 20) while the model, its selection collider and the sim entity stay intact and clickable.
- Hangar roll-door now moves the single authored door (no runtime split/scale); strict open-before-drive handshake, never clips through a closed door.
- Fixed the heliport collider warning and the helicopter becoming unclickable (asset bundles were silently stripped of colliders/particles - Unity physics/particle modules re-added and bundles force-rebuilt).
- Fixed duplicate prototype registration ("equal IDs") and the sling re-render AmbiguousMatch error.

## 0.5.3

- Helicopters no longer reserve the exclusive ground-vehicle queue at cargo buildings.
- Multiple helicopters can now load, unload and refuel at the same building simultaneously.
- Regular trucks and every non-helicopter vehicle continue to use the vanilla queue unchanged.
- Wider per-aircraft altitude lanes remain active during transfer to reduce rotor/model overlap.
- Existing queued jobs remain save-safe and finish once; newly dispatched jobs use parallel service.

## 0.5.2

- Fixed wood cargo using the generic container instead of the log sling (`Product_Wood` ID handling).
- Cleared all vehicle-id keyed landing, cargo-ground and next-load state on save changes and despawn.
- Removed temporary per-helicopter cargo/display diagnostic log spam.
- Avoided a misleading `HasTrueJob` warning during the renderer/job-queue initialization edge.
- Added safe high-speed terrain waypoint handling for helicopter routes.
- Added a compatibility fix for the truck light controller's material lookup on the helicopter prefab.
- Added idle return to the nearest vehicle depot.
- Synchronized manifest, assembly and file versions.
- Made builds/deployments include ParticleSystemModule, all three icons and every AssetBundle.
- Added a reproducible release script and validated package layout.

## 0.5.1

- Idle helicopters only land on dry, open and unoccupied ground.
- Helicopters over water or buildings remain at a safe hover altitude.
- Added job hysteresis to stop idle landing/takeoff oscillation.
- Empty idle helicopters reel the sling tight against the hull.
- Idle hovering uses the vanilla parked-vehicle fuel behavior.

## 0.2.0

Big update, focused on **how it flies** and **how it looks**. Huge thanks to everyone testing — and especially **@Vladovlak**, whose screenshots and notes drove most of this. 🙌

## Replying to your points, Vladovlak:
- 🌀 **Rotor** — you were right, it was basically a war machine. It's now **6 blades at ~half the diameter**, and spins correctly.
- 🌊 **"Turning into a submarine"** — fixed. Over deep water it no longer sinks below the surface; it flies **above the water** now.
- 🍿 **"Jumping over the bridge like popcorn"** — this was the big one, and the altitude is **completely reworked**. The heli now looks at the **highest point along its route ahead**, climbs to it **gradually while flying** (no more last-second leaps), holds a **steady height** over the mountains/stacks on the way, and **descends smoothly** as it nears the destination. Ascending/descending is gentle now, not a rocket. 🚁

## Full changelog
- ✈️ **Flight altitude fully rewritten** — smooth, route-aware; no more popcorn/jumping over mountains, cliffs, bridges or tall buildings
- 🌊 Flies **above water** instead of submerging
- 🏭 Climbs **over tall buildings / smokestacks** instead of clipping through them
- 🌀 New rotor: 6 blades, correct size
- 📦 Slung cargo is now **frame-locked** to the heli — the load stopped jittering under it
- 🖱️ You can now **click the helicopter itself** to select it (added a proper hitbox)
- ⏸️ On **pause** it freezes correctly now (no drifting, rotor stops)
- 🎨 **De-militarized** (weapons removed) + new civilian paint
- 🖼️ Custom **icon** in the vehicle factory
- ⚡ **Faster** and a **higher** cruise altitude
- 🔕 Hides the false *"building can't be reached by vehicles"* warning while a heli is around

Still a **test build** — please keep the feedback coming, it genuinely shapes the mod. If anything still looks off (altitude, cargo, the model), drop a screenshot and I'll fix it. Thanks again! 🚁
## 0.9.0

- Replaced the Heliport Complex with the new marker-authored model and a new transparent game icon.
- Added four exact cargo positions with five owned stack levels (20 stored attachments).
- Rebuilt arrival/departure choreography: direct cargo drop, short pad approach, smooth landing, gate taxi and direct job release without scripted circles or teleport landing.
- Added atomic sling/yard visual handoff to remove duplicate containers, rotation snaps and floating yard cargo.
- Made flight, winch, rotor and sling simulation camera-independent and frame-time correct.
- Added five altitude lanes with predictive air-traffic separation for ordinary flights.
- Fixed off-screen winch/landing stalls, stale tank/container selection, repeated route restarts and premature door binding.
- Updated assembly lifts, doors, markers, footprint and selection colliders for the new building.
