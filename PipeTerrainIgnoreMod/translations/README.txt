Underground Pipes translations
==============================

Each JSON file contains one language and is encoded as UTF-8. The filename
must match Captain of Industry's language filename, for example en.json,
de.json, pt_BR.json, zh_Hans.json, or zh_Hant.json.

Editing rules
-------------

- Edit values on the right side only. Do not rename the keys on the left.
- Keep {0} inside DepthFormat; the mod replaces it with the selected depth.
- Text inside <b>...</b> receives the larger yellow emphasis used by the
  demolition and copy confirmation windows.
- Missing, empty, unknown, or invalid entries fall back to English, so a
  translation mistake cannot prevent the mod or a save from loading.
- Restart the game after editing a file. Translations are cached per language
  while the game is running.

Update-safe personal overrides
------------------------------

Files inside the mod's translations folder can be replaced by a mod update.
To keep personal corrections across updates, create this folder:

  %APPDATA%\Captain of Industry\ModConfigs\PipeTerrainIgnoreMod\translations

Place a JSON file with the same language name there. Override files may
contain only the keys you want to change. Their values are loaded after the
files shipped with the mod.
