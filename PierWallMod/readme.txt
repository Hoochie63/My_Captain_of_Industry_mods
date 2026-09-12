CAP Pier Wall
=============

A mod for "Captain of Industry" that adds retaining walls that can be placed
in the ocean.

Tested up to: 0.8.7a

New V1.0.2
----------

- Console Command
	- SetHeightOffset(int value): Sets the height offset for the pier walls.
	- GetHeightOffset:            Gets the current height offset for the pier walls.
	- SetCategory(bool value):    Specifies whether the buildings should be assigned to a mod-specific category.
	- GetCategory:                Gets the value of the mod-specific category setting.
- Add Config
    - CollisionHeightOffset:      Specifies the height offset for the pier walls. Default is 0.
    - SortInCategory:             Specifies whether the buildings should be assigned to a mod-specific category. Default is true.

How to Build over PierWalls
---------------------------

If you want to build over the Pier Walls, you can use the SetHeightOffset console command to adjust the height of the collision box. For 
example, if you want to build a trading dock over the Pier Walls, you Set the height offset to -20 and save and reload the savegame. Than 
you can build the trading dock over the Pier Walls. After that you can reset the height offset to 0 and save and reload the savegame again. 

It's not Required for the Pier Walls to work that the Height is set to 0.

**Attention:** When two Buildings are placed in the same place it is possible that you can not reach one of the buildings anymore.

Features
--------

- Adds five pier wall variants:
  - Pier Wall (short)
  - Pier Wall (long)
  - Pier Wall (corner)
  - Pier Wall (cross)
  - Pier Wall (tee)

- Can be placed on land and in the ocean.
- Uses the standard retaining wall graphics.
- Costs exactly the same as vanilla retaining walls.
- Adds a research node to unlock the pier walls.


Installation
------------

Extract the mod ZIP to:

%APPDATA%\Captain of Industry\Mods\PierWallMod

The mod folder should contain:

- manifest.json
- config.json
- PierWallMod.dll
- AssetBundles/
- README.txt
- thumbnail.png


Compatibility with Save Games
-----------------------------

This mod can be added to an existing save game.

Removing the mod from an existing save file is not supported, as the save file
may contain pier wall entities registered by the mod.


Note on AI Support
------------------

Parts of this mod were developed with the support of ChatGPT / GPT-5.5 Thinking
from OpenAI.


License
-------

This mod is licensed under the Captain of Industry Open License (COI-Open) 1.0.

See the LICENSE file for details.
