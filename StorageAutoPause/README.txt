Storage Auto Pause v1.9.1

FINAL EFFECT AND LOGIC PASS IN 1.9.1
- Logistics trend memory independently shows filling green upward and draining cyan downward; an active rule is stable red.
- Player pause/unpause keeps ownership of the current active cycle; automation rearms only after the condition clears.
- Diagnostic ring and vertical logistics animations use independent phases.
- The breakdown flare flies roughly three times higher, has a thin smoke trail, an apex flash and a small smoke burst.
- Spatial sound is about 25% stronger but remains distance-limited.
- Only a broken vehicle emits the additional small black smoke puff at ground level; buildings do not.

FIXES AND BREAKDOWN FIREWORKS IN 1.9.0
- Every external player pause/unpause now invalidates marker visuals even when automation ownership does not change.
- Manual pause is blue immediately; unpause returns to the correct rule state without removing/readding storage.
- Player unpause during a TRUE rule owns the current cycle; automation rearms only after FALSE and never fights the player.
- Tower panel body now uses the full inspector content width instead of collapsing controls to the left.
- Newly broken maintained buildings launch one independent warning firework per broken-state transition.
- Firework follows a randomized shallow parabola, leaves a launch trail, bursts into 95 sparks, falls under gravity, and fades over 3-5 seconds.
- Existing broken buildings are primed silently and all transitions are suppressed for 60 seconds after mod initialization.
- Spatial launch/explosion audio is enabled by default with logarithmic attenuation (7-90 world-unit range).
- Fireworks and sound can be disabled independently in config.json.

AUDIO
- Whistle and Explosion Single_Firework by Rudmer_Rotteveel, Freesound sound 336008.
- Creative Commons Zero (CC0 1.0), preview OGG bundled unmodified.
- Source: https://freesound.org/people/Rudmer_Rotteveel/sounds/336008/

STATE-MACHINE FIXES IN 1.8.7
- Manual pause always belongs to the player and is never released by logistics.
- Manual unpause during an already-active logistics condition suppresses automation for that condition cycle.
- Automation rearms after the condition becomes false; only a later true condition can pause again.
- Rules, selected storages and thresholds remain configured during manual ownership.
- Worker warning no longer treats HasWorkersCached=false alone as missing workers. The game can still supply workers through WorkersManager.CanWork.
- Worker light now turns orange only for the entity's explicit NotEnoughWorkers or MissingWorkers operational state.

FIXES IN 1.8.6
- Fixed empty Mine Tower and Forestry Tower panels: content now uses the game's real Column.Add API instead of nonexistent Append.
- Manual pause/unpause no longer disarms an active logistics rule. Automation re-evaluates and reclaims control on the next simulation update.
- Existing saved PlayerOverrideUntilClear values no longer block automation.
- Electricity and worker rings use eight solid wedge segments instead of twelve surface-only segments.
- Each wedge now has top, bottom, outer and radial side faces.
- Inactive segments use a visible dark housing material, so the lamp never looks hollow.
- Removed shared brightness pulsing that could race against positional animation and produce out-of-order flashes.

FIXES AND VISUAL UPDATE IN 1.8.5
- Any storage can now be selected for a factory rule; recipe commodity filtering no longer rejects valid player-designed links.
- Mining and forestry control towers are accepted by the semaphore feature.
- Injected control-tower panels are moved to the top of the inspector body, including inherited MainBody fields.
- Electricity and worker diagnostics stay green while an entity is paused and for resources the entity does not use.
- Pure semaphore mode uses a shorter mesh and completely omits the dark logistics stub.
- Logistics is a ten-stage vertical indicator: normal green, approaching threshold animated orange upward, automation pause red, manual pause blue.
- Worker and electricity lamps are twelve-segment rings. Warning/failure light travels around the circumference in opposite directions.
- Marker objects remain on Unity's Ignore Raycast layer and cannot block structure selection.

FIXES IN 1.8.4
- Mine Tower automation panel is moved to the top of the inspector so it is always visible.
- Electricity and worker lights default to green when that resource is not used.
- Diagnostic state is held stable while an entity is paused; logistics no longer creates false power/worker warnings.
- Player unpause overrides automation until the rule clears instead of being immediately undone.
- Pause ownership, hysteresis and player overrides now survive save/load.
- One Ore Sorting Plant can belong to only one Mine Tower automation rule.
- UI and simulation rule access is synchronized to prevent functions overwriting each other.
- Runtime rule-state persistence is deferred safely to the UI update.

FIXES IN 1.8.3
- Fixed structure clicks being swallowed by the injected inspector panel.
- Mod UI now clears only its own dedicated child container, never a native inspector panel body.
- Semaphore antennas use Unity's Ignore Raycast layer and cannot participate in world picking.
- EntityPauseStateChanged is correctly unsubscribed when the mod is disposed.

Author: Sirael
Contact: alena.verabei@protonmail.com
IBAN: BE94967304158014
Copyright © 2026 Sirael. All rights reserved.

Automatic factory control based on connected storage levels.

- Compact horizontal native COI automation UI.
- Storage summary cards + ELSE tile + one-row actions.
- Detailed threshold sliders appear only after UPRAVIT.
- Automation rules are integrated with the save.
- Shared 3D lattice antenna markers with optimized rendering.


v1.8.0
- Fixed orange logistics warning transition detection.
- Added Mine Tower logistics with manually selected Ore Sorting Plants.
- Added three-light semaphore antenna: logistics / electricity / workers.
- Electricity: green OK, synchronized pulsing orange when power is missing.
- Workers: green OK, synchronized pulsing orange when workers are missing.
- Maintenance breakdown overrides both lower lights with opposite-phase pulsing red.
- Semaphore appears automatically with logistics; it can also be enabled manually per supported building.
- Added diagnostic_semaphore_enabled mod setting.


v1.8.2
- Fixed electricity light on paused/idle electricity-powered buildings.
- Worker light now uses IEntityWithWorkers.HasWorkersCached as the authoritative assignment signal.
- Fixed duplicated/separator panels by caching one injected panel per concrete inspector instance.
- Mine Tower and other supported inspectors use inherited public/non-public panel factories.
- Added change-only semaphore diagnostics to the game log for verification without per-frame spam.
