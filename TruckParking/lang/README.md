# Truck Parking translations

Truck Parking resolves all player-facing text through MultiLangLib. Each file
in this directory is a UTF-8 JSON object whose keys are stable text IDs. At
runtime, for example, `toolbar.name` becomes the canonical key
`multilanglib.TruckParking.toolbar.name`.

## Included locales

The directory includes catalogs for all 21 official Captain of Industry locale
filenames: `ca`, `cs`, `de`, `en`, `es`, `et`, `fr`, `hu`, `it`, `ja`, `ko`,
`nb_NO`, `nl`, `pl`, `pt_BR`, `ru`, `sv`, `tr`, `uk`, `zh_Hans`, and
`zh_Hant`. The additional neutral `pt` catalog is a fallback for Portuguese
regional locales; it does not represent a separate game language.

## Adding or updating a language

1. Copy `en.json` to the Captain of Industry language filename you want to
   support, for example `it.json` or `pt_BR.json`.
2. Translate values only. Keep every JSON key unchanged.
3. Preserve all numbered placeholders such as `{0}`, `{1}`, and `{4}`. Their
   order may change to match the target language, but none may be omitted from
   a format string unless the English entry also leaves it unused.
4. Save the file as valid UTF-8 JSON and keep exactly the same key set as
   `en.json`.
5. Start the game with MultiLangLib and Truck Parking enabled. MultiLangLib's
   `debug_language` option shows canonical keys in the UI and helps locate any
   untranslated element.

MultiLangLib checks a regional file before its neutral language. For example,
`pt_BR.json` overrides `pt.json`; if the regional file is absent, `pt.json` is
used. Missing translations fall back according to MultiLangLib's configured
fallback language, which is English by default.

## Vehicle-filter semantics

An empty vehicle filter is the safe default: it allows all supported ground
vehicles while blocking detected aircraft. Once the player explicitly selects
vehicle types, that selection is authoritative and can opt aircraft in. The
clear action restores the safe empty-filter default and removes stale saved IDs
belonging to vehicle mods that are no longer installed. Translations of
`inspector.filter.tooltip` and `inspector.filter.clear.tooltip` must preserve
this distinction.

## Plural and grammar keys

- `slot.singular`, `slot.few`, and `slot.many` represent the 1, 2–4, and other
  numeric forms. Languages with only singular/plural forms should give `few`
  and `many` the same plural value.
- `area.description.singular` is used for one slot; the plural entry is used
  for every other supported slot count.
- `area.variant_size.*` may differ grammatically from `size.*.name` because it
  appears inside a full parking-area name.

## Placeholder contracts

| Key | Placeholders |
| --- | --- |
| `area.name` | `{0}` base name, `{1}` size, `{2}` count, `{3}` slot word, `{4}` style |
| `area.description.*` | `{0}` count, `{1}` slot word, `{2}` length, `{3}` width, `{4}` maintenance text |
| `error.parking_area_not_found` | `{0}` parking-area ID |
| `error.vehicle_prototype_unsupported` | `{0}` vehicle-prototype ID |

The preferred compact layout for `area.name` is
`{0} – {2} {3} – {1} ({4})`: base name, slot count, size, then style.

## Deutsch

Für eine neue Übersetzung `en.json` unter dem passenden COI-Sprachdateinamen
kopieren, nur die Werte übersetzen und alle Schlüssel sowie nummerierten
Platzhalter beibehalten. MultiLangLib verwendet standardmäßig Englisch als
Fallback. Der Debugmodus von MultiLangLib zeigt im Spiel die vollständigen
Schlüssel an.
