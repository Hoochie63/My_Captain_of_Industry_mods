# MultiLangLib für Captain of Industry

MultiLangLib ist eine gemeinsame Übersetzungsbibliothek für Captain-of-Industry-Mods. Ein Consumer übergibt einen stabilen Schlüssel, zum Beispiel:

```csharp
string title = Lang.Get("multilanglib.MyMod.menu.title");
LocStrFormatted label = Lang.Localized("multilanglib.MyMod.menu.title");
string greeting = Lang.Format("multilanglib.MyMod.welcome", playerName);
```

Die Library liest den Text aus dem `lang`-Verzeichnis des aufrufenden Mods oder aus ihrem zentralen Fallback-Verzeichnis. Im Debugmodus gibt sie exakt den Schlüssel aus, also beispielsweise `multilanglib.MyMod.menu.title`.

Die Version 0.1.0 ist gegen Captain of Industry 0.8.6c (Build 612) und .NET Framework 4.8 gebaut. Die Modding-API ist laut MaFi Games weiterhin experimentell; neue Spielversionen sollten deshalb erneut getestet werden.

## Wichtig: Schlüssel müssen über das API aufgelöst werden

Eine nackte Zeichenkette wie `"multilanglib.MyMod.menu.title"` wird vom Spiel nicht automatisch ersetzt. Der Aufruf muss über `Lang.Get(...)`, `Lang.Localized(...)` oder `Lang.Format(...)` erfolgen. So bleibt die Library ohne Harmony, Reflection in private Engine-Daten und andere versionsfragile Patches.

## Installation

Das Release-ZIP wird direkt nach `%APPDATA%\Captain of Industry\Mods` entpackt. Das Archiv enthält den Ordner `MultiLangLib` bereits als Wurzel; dadurch entsteht folgende Struktur:

```text
%APPDATA%\Captain of Industry\Mods\MultiLangLib\
├── MultiLangLib.dll
├── manifest.json
├── config.json
├── readme.txt
├── README.md
├── LICENSE
└── lang\
```

Der Ordnername muss `MultiLangLib` bleiben. Anschließend wird MultiLangLib im Mod-Manager zusammen mit den Consumer-Mods aktiviert.

## Sauberes Upload-Paket

Das Release enthält nur die zur Nutzung und Dokumentation benötigten Dateien. Quellcode, PDB-Dateien, XML-API-Dokumentation sowie generierte Verzeichnisse wie `bin`, `obj`, `.git` und `artifacts` bleiben außerhalb des Upload-ZIPs.

## Consumer-Mod anbinden

Der Consumer erklärt MultiLangLib als verpflichtende Abhängigkeit. Ohne Leerzeichen um `>=` ist der Eintrag sowohl mit dem aktuellen Loader als auch mit dem offiziellen JSON-Schema kompatibel:

```json
{
  "id": "MyMod",
  "version": "1.0.0",
  "primary_dlls": [ "MyMod.dll" ],
  "mod_dependencies": [ "MultiLangLib>=0.1.0" ]
}
```

Beim Kompilieren wird auf die installierte `MultiLangLib.dll` verwiesen, ohne sie in den Consumer-Mod zu kopieren:

```xml
<Reference Include="MultiLangLib">
  <HintPath>$(APPDATA)\Captain of Industry\Mods\MultiLangLib\MultiLangLib.dll</HintPath>
  <Private>false</Private>
</Reference>
```

Innerhalb des Quellprojekts zeigt `examples/ExampleConsumer/ExampleConsumer.csproj` stattdessen direkt auf das MultiLangLib-Projekt. Der Beispielmod registriert sein Root-Verzeichnis ausdrücklich:

```csharp
public ExampleConsumerMod(ModManifest manifest) : base(manifest) {
    Lang.RegisterMod(manifest.Id, manifest.RootDirectoryPath);
}
```

Diese Registrierung ist empfohlen, aber für normal installierte Geschwisterordner nicht zwingend: MultiLangLib kann `%APPDATA%\Captain of Industry\Mods\<ModId>` selbst finden.

## Schlüssel

Das kanonische Format lautet:

```text
multilanglib.<ModId>.<TextId>
```

Beispiele:

```csharp
Lang.Get("multilanglib.MyMod.window.title");
Lang.Get("MyMod", "window.title");
Lang.Localized("MyMod", "window.title");
```

- `<ModId>` beginnt mit Buchstabe oder Ziffer und enthält Buchstaben, Ziffern, `_` oder `-`.
- `<TextId>` darf zusätzlich Punkte für eine lesbare Hierarchie enthalten.
- Groß-/Kleinschreibung ist bei IDs absichtlich relevant.
- Punkte in `<ModId>` sind nicht erlaubt, weil sonst die Grenze zwischen Mod- und Text-ID mehrdeutig wäre.

## Sprachdateien und Suchreihenfolge

Bei deutscher Spielsprache sucht MultiLangLib für `multilanglib.MyMod.window.title` in dieser Reihenfolge:

```text
1. <MyMod>/lang/de.json
2. <MultiLangLib>/lang/MyMod/de.json
3. <MyMod>/lang/en.json
4. <MultiLangLib>/lang/MyMod/en.json
5. Schlüssel: multilanglib.MyMod.window.title
```

Bei einem sprachspezifischen Namen wie `de-DE` wird zuerst `de-DE.json` und danach `de.json` geprüft. Die echten COI-Dateinamen (`pt_BR.json`, `zh_Hans.json` und so weiter) werden im Automatikmodus direkt aus `LocalizationManager.CurrentLangInfo.FileName` übernommen.

Auch ein wörtlicher Dateiname `language.json` ist möglich: Dazu wird `language_override` auf `language` oder `language.json` gesetzt.

## JSON-Formate

Empfohlen ist ein einfaches Objekt. Innerhalb des Mod-Verzeichnisses reicht die Text-ID:

```json
{
  "window.title": "Produktionsübersicht",
  "welcome": "Willkommen, Captain {0}!"
}
```

Alternativ wird das etablierte COI-Arrayformat akzeptiert. Die ID kann kurz oder vollständig sein:

```json
[
  [ "multilanglib.MyMod.window.title", "Produktionsübersicht" ],
  [ "welcome", "Willkommen, Captain {0}!" ]
]
```

Weitere Arraywerte werden für künftige Pluralunterstützung toleriert; Version 0.1.0 verwendet beim normalen Lookup den ersten Übersetzungstext. Platzhalter werden mit `Lang.Format(...)` und der aktiven Sprachkultur formatiert.

Ist ein Platzhalter in einer Übersetzung fehlerhaft oder fehlt ein Argument, protokolliert MultiLangLib die betroffene ID einmalig und liefert den unformatierten Text zurück. Ein einzelner Übersetzungsfehler bricht dadurch keinen UI-Aufruf ab.

## Debugsprache

Das öffentliche Spiel-API erlaubt keine stabile Erweiterung der eingebauten Sprachenliste. Deshalb stellt MultiLangLib den Debugmodus als eigene Mod-Einstellung bereit:

```text
debug_language = true
```

Danach gilt für jeden gültigen Lookup:

```csharp
Lang.Get("multilanglib.MyMod.window.title")
// -> multilanglib.MyMod.window.title
```

`language_override` hat zusätzlich folgende Werte:

- `auto`: folgt der im Spiel gewählten Sprache.
- `debug`: zeigt wie `debug_language = true` die vollständigen Schlüssel.
- `de`, `de-DE` oder `de.json`: erzwingt eine Sprachdatei.

`fallback_language` ist standardmäßig `en`. Fehlende oder ungültige Übersetzungen werden einmalig ins COI-Log geschrieben; als sichtbarer Wert bleibt der vollständige Schlüssel erhalten.

Sprachdateien werden zwischengespeichert. Entwicklungswerkzeuge können nach Dateiänderungen `Lang.Reload()` aufrufen. Bereits gerenderte UI-Elemente aktualisiert das Spiel dadurch nicht automatisch.

## Bauen und testen

Voraussetzungen:

- Captain of Industry installiert
- .NET SDK 9.0.200 oder neuer (für `.slnx`) mit Zugriff auf .NET Framework 4.8 Reference Assemblies
- optional `COI_ROOT` mit dem Spielverzeichnis

Der Build-Helfer erkennt die Standard-Steam-Installation automatisch:

```powershell
.\build.ps1
```

Mit explizitem Pfad:

```powershell
.\build.ps1 -CoiRoot 'D:\SteamLibrary\steamapps\common\Captain of Industry'
```

Das Skript baut MultiLangLib und den ExampleConsumer, führt den eigenständigen Test-Harness aus und erzeugt:

```text
artifacts\MultiLangLib_0.1.0.zip
```

Optional installiert `-Deploy` MultiLangLib nach `%APPDATA%\Captain of Industry\Mods\MultiLangLib`. Es werden keine anderen Mod-Verzeichnisse verändert.

Mit `-ModsRoot` lässt sich ein anderes Ziel angeben, beispielsweise ein sauberer Upload-Arbeitsordner:

```powershell
.\build.ps1 -Deploy -ModsRoot 'C:\coi\mods'
```

`-Deploy` ersetzt ausschließlich den Zielordner `MultiLangLib` vollständig durch den frisch gebauten Paketinhalt. Andere Mod-Verzeichnisse bleiben unverändert.

Die Tests decken lokale und zentrale Dateien, Sprach- und Englisch-Fallbacks, beide JSON-Formate, Debugausgabe, Cache-Reload, Formatierung, `LocStrFormatted` und die gemeinsame DLL-Bindung eines Consumer-Mods ab.

## Projektstruktur

```text
src/MultiLangLib/                 Library und COI-Mod-Entry-Point
examples/ExampleConsumer/   kompilierbarer Consumer-Mod
tests/MultiLangLib.Tests/        paketfreier net48-Test-Harness
build.ps1                   Build, Tests und Release-Paket
```

Grundlage sind das [offizielle Modding-Repository](https://github.com/MaFi-Games/Captain-of-industry-modding) und die [Captain-of-Industry-Modding-Policy](https://www.captain-of-industry.com/modding-policy).
