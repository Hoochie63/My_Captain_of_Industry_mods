CAP Pier Wall Seawater Pump
===========================

A mod for "Captain of Industry" that adds a seawater pump that must be built on
CAP Pier Wall support tiles.

Version: 1.0.0
Tested up to: 0.8.6a
Requires: PierWallMod


Features
--------

- Adds a pier-wall seawater pump for pumping seawater from the ocean through a
  CAP Pier Wall.
- Requires CAP Pier Wall support under the marked 2 x 4 support area.
- Works with compatible CAP Pier Wall variants:
  - Pier Wall (short)
  - Pier Wall (long)
  - Pier Wall (corner)
  - Pier Wall (cross)
  - Pier Wall (tee)
- Supports placement across multiple pier wall segments, so the support area can
  span segment seams.
- Prevents supported pier wall tiles from being removed while a pier-wall
  seawater pump depends on them.
- Adds a research node to unlock the pier-wall seawater pump after the CAP Pier
  Walls research.
- Uses a custom model and custom icon.
- Uses the vanilla Ocean Water Pump T1 construction cost and base power behavior.
- Includes normal and high-throughput seawater pumping recipes.


Supported Pumping Recipes
-------------------------

- Seawater pumping: outputs 18 Seawater every 10 seconds.
- Seawater pumping (2x): outputs 18 Seawater every 5 seconds at 300%
  power.


Installation
------------

Extract the mod ZIP to:

%APPDATA%\Captain of Industry\Mods\CAP-PierWall-SeawaterPump

Make sure the required dependency PierWallMod is installed and enabled.

The mod folder should contain:

- manifest.json
- config.json
- CAP.PierWall.SeawaterPump.dll
- AssetBundles/
- README.txt
- thumbnail.png


Compatibility with Save Games
-----------------------------

This mod can be added to an existing save game.

Removing the mod from an existing save file is not supported, as the save file
may contain pier-wall seawater pump entities registered by the mod.


Configuration
-------------

This mod currently has no custom configuration options.


Build from Source
-----------------

The project targets .NET Framework 4.8 and references the Captain of Industry
managed assemblies.

To build locally:

1. Set COI_ROOT to your Captain of Industry installation directory.
2. Open CAP.PierWall.SeawaterPump.sln or CAP.PierWall.SeawaterPump.slnx.
3. Build the project.

When deployment is enabled, the build process copies the mod files to:

%APPDATA%\Captain of Industry\Mods\CAP-PierWall-SeawaterPump


Note on AI Support
------------------

Parts of this mod were developed with the support of ChatGPT / GPT-5.5 Thinking
from OpenAI.


License
-------

This mod is licensed under the Captain of Industry Open License (COI-Open) 1.0.

See the LICENSE file for details.
