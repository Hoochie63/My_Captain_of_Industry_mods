MULTI-PRIORITY BALANCERS

Adds four balancer sizes for every product transport:
- Flat conveyor
- U-shape conveyor
- Pipe
- Molten channel

All variants are grouped in the dedicated P1/P2/P3 Balancers category and
use explicit names so they cannot be confused with standard balancers.

Click a connected input or output port to cycle:
Normal -> P1 -> P2 -> P3 -> Normal

P1 is saturated first. P2 is only used when P1 cannot accept or provide
anything more, followed by P3, then normal ports. Ports on the same level
are served in round-robin order. Enabling equal input/output ratios clears
priorities in that direction.

The priority levels are stored in saved games. This mod cannot be removed
from a save after its custom balancers have been built.

VISUAL ASSET CREDIT
The additional balancer model and icon bundle comes from Industrial
Expansion 1.0.5 by Luminiel and is reused under CoI-Open 1.0:
https://coigame.com/Mod/1097/Industrial-Expansion

Industrial Expansion code and unrelated assets are not included. This mod
uses its own mpb_balancers asset bundle and works both by itself and alongside
Industrial Expansion. Industrial Expansion is not a required dependency.
Version 0.2.1 also gives the bundle its own Unity CAB and streamed resource
references to prevent missing icons and black materials when both mods load.

LANGUAGE
The mod works with every game language. Building names and descriptions use
English fallback text. P1, P2 and P3 are language-independent.
