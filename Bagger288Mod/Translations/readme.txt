Bagger 288 — Translations
=========================

All player-facing strings for this mod live in this folder.
The mod loads en.json first, then overlays the file for the
current game language (if present).

How to add or update a language
-------------------------------
1. Copy en.json and rename it to the game language file name
   (see list below), e.g. de.json, fr.json, ja.json.
2. Translate only the second string in each [ "Key", "Text" ] pair.
   Do not change the keys.
3. Keep valid JSON (UTF-8). Trailing commas are not allowed.
4. Place the file in this Translations folder next to the mod DLL.
5. Restart the game and switch to that language in settings.

Format (same as vanilla Captain of Industry):

[
	[
		"SomeKey",
		"Translated text"
	],
	[
		"AnotherKey",
		"More text"
	]
]

Keys used by this mod
---------------------
- Bagger288__name / Bagger288__desc — machine name and description
- UnlockBagger288__name / UnlockBagger288__desc — research node
- Bagger288Mod__* — inspector UI labels
- Bagger288Mod__YawLimitLeft / YawLimitRight / ShowYawArc — dig-arc limits and overlay
- Bagger288Mod__UseExtendedPorts — wider rear ports model (shifts C/D)
- Bagger288Mod__TileUnit0/1/2 — plural forms of "tile" (language plural index)

Game-supported languages (file name)
------------------------------------
en.json      English (en-US)
ca.json      Català (ca-ES)
cs.json      Čeština (cs-CZ)
de.json      Deutsch (de-DE)
es.json      Español (es-ES)
et.json      Eesti keel (et-EE)
fr.json      Français (fr-FR)
hu.json      Magyar (hu-HU)
it.json      Italiano (it-IT)
ja.json      日本語 (ja-JP)
ko.json      한국어 (ko-KR)
nb_NO.json   Norsk bokmål (nb-NO)
nl.json      Nederlands (nl-NL)
pl.json      Polski (pl-PL)
pt_BR.json   Português brasileiro (pt-BR)
ru.json      Русский (ru-RU)
sv.json      Svenska (sv-SE)
tr.json      Türkçe (tr-TR)
uk.json      Українська (uk-UA)
zh_Hans.json 简体中文 (zh-CN)
zh_Hant.json 繁體中文 (zh-Hant)

This mod currently ships: en.json, ru.json.
