PROCEDURAL RESEARCH TREE
============================

Automatically prevents vanilla and modded research cards from overlapping while
preserving the progression and visual intent authored by each mod.

HOW IT LAYS OUT THE TREE
------------------------

- Vanilla research is treated as the fixed backbone.
- Connected research branches belonging to a mod retain their internal shape.
- Three grid units exactly match the game's research-card width and height, so
  this version no longer adds the unnecessary empty row used by older builds.
- When a branch collides, compact placement first tries legal space inside the
  vanilla tree's existing vertical envelope.
- Packed placement keeps the chosen clearance but tests every individual grid
  row, allowing safe spacing 3 to use legal offsets that Compact deliberately
  skips in three-row increments. It also prefers contact with an occupied band
  before leaving avoidable blank rows.
- A mod branch is never placed above the fixed vanilla tree's authored top edge.
- If an author placed two connected cards on top of one another, only that
  malformed internal collision is repaired before the branch is placed.
- Horizontal coordinates are preserved, including intentionally non-linear paths.
- Parent relationships, costs, unlocks, conditions, and save progression are not
  modified.

COMPATIBILITY
-------------

No special API or cooperation from other mod authors is required. The mod reads
all normal ResearchNodeProto registrations after the prototype database is
complete and applies the layout once during dependency registration.

RESEARCH REQUIREMENTS
---------------------

The REQUIRES panel appends each node's immediate parents: exactly one connection
back, never the entire recursive prerequisite chain. Existing research-lab and
special unlocking requirements remain visible.

These added parent tiles are clickable. Clicking one selects that parent research
and smoothly centers the research-tree camera on its card. The game's existing
research-lab and special requirement tiles are not modified.

A REQUIRED BY section after the game's queue/status controls lists every immediate
child of the selected research. Its tiles use the same click-to-select and center
behavior, providing forward navigation and an easy route back after following a
parent.

The research toolbar can disable or re-enable both PRT navigation sections live.
This switch affects only PRT-created tiles. Vanilla lab-tier and special-condition
tiles, plus requirement UI authored by another mod, remain untouched. PRT compares
the game's baseline requirement row with the final row after other patches and
dynamically defers when another mod supplied its own presentation; no third-party
mod names or node IDs are hard-coded.

This display enhancement uses two ordered Harmony postfixes on the research-detail
panel. The layout itself still runs only once through the public mod lifecycle and
does not patch research-window construction.

The same set of installed mods always produces the same layout regardless of
prototype enumeration order.

NODE OWNERSHIP HIGHLIGHTING
---------------------------

The Layers icon beside the layout control can emphasize all mod research,
vanilla and official-DLC research, or the research belonging to one dynamically
discovered mod. Selecting any filter turns the Layers button gold and reveals
one compact, unlabeled opacity slider beside the icon controls. The slider dims
all other cards from 0% through 50%, with 20% as the default; 0% makes them fully
invisible while the filter is active. Show All restores every card to full
opacity, clears the gold active state, and hides the slider completely. Choices
come from the research nodes that actually exist; no per-mod names or IDs are
hard-coded into this feature. The unsuccessful experimental green-glow control
from 0.2.2h has been removed completely.

LIVE TREE DIAGNOSTICS
---------------------

Enter PRT_tree in the in-game console to write a complete, read-only snapshot
of the currently registered research graph to the normal game log. The dump
contains authored and final bounds plus every node's ID, owner, original and
current coordinates, direct parents, direct children, and diagnostic flags.
The command never reapplies the layout and never changes research state.

LIVE LAYOUT PREVIEW
-------------------

The research window's bottom control strip now includes Captain of Industry's
native configure icon immediately after the Search field. Its native dropdown
exposes Original first, followed by Compact 1-4, Nearest 1-4, and Packed 1-4.
Compact spacing 3 is explicitly marked as the PRT default. Spacing 1 and 2 are
clearly marked as
experimental because research cards can physically overlap at those distances.
Selecting an entry rebuilds the open tree immediately. Spacing 5 was removed
because stress testing showed that it could scatter nodes far outside a sensible
tree layout.

All PRT-owned labels and tooltips use editable English-text keys and ship with
catalogs for all 21 supported game languages. Research names, descriptions, and
requirement cards continue to use the localization supplied by the game or the
mod that owns each prototype.

SPECIAL THANKS
==============
Special thanks to Colibri, author of Colibri Industries, for helping with mod
development.

Layout choices are session-only and are never written to a save or config file.
PRT_tree is the only console command shipped; it remains available strictly for
read-only troubleshooting and never changes the research tree.

PACKAGE IDENTITY
----------------

- Display name: Procedural Research Tree
- Author: X-Mag-X
- Mod ID, installation folder, and ZIP root: procedural-research-tree
- Runtime DLL: ProceduralResearchTree.dll

RELEASE AND INSTALLATION
------------------------

Test 0.2.2d: adds dynamic research-node ownership highlighting beside the
translated layout dropdown. It can emphasize all mods, vanilla/official DLC, or
one installed mod while leaving dimmed cards visible and clickable.
Install only ONE version at a time. Extract the procedural-research-tree folder
into Captain of Industry's Mods folder. When replacing a previous installation,
remove the former mod folder first so both identities/versions are not loaded.
This package does not include source code; its matching versioned source folder
is retained separately for building and review.
