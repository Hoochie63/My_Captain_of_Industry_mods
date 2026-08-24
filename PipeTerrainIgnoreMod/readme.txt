Underground Pipes 1.2.1
=======================

Dieser Mod erlaubt es, Rohre durch das Gelaende zu bauen.

Mit der normalen Taste beziehungsweise Schaltflaeche "Transport absenken"
kann die Rohr-Bauhoehe nun auf -1 bis -6 gestellt werden. Die Hoehe ist
relativ zur lokalen Gelaendeoberflaeche; -6 liegt sechs Felder darunter.

Betroffen sind ausschliesslich Rohrtransporte (Rohr I-IV) sowie deren
Rohrverbinder und Rohr-Ausgleicher:
- Die Laufzeit-Anbindung ist mit den in gaengigen Mods enthaltenen Harmony-
  Versionen 2.2.2 bis 2.4.2 kompatibel. Auch wenn ein anderer Mod Harmony
  zuerst laedt, bleiben Rohrvorschauen, andere Transporte und Anschluesse
  funktionsfaehig.
- Alle sichtbaren Mod-Texte folgen automatisch der im Spiel gewaehlten
  Sprache. Alle mit Captain of Industry 0.8.7 ausgelieferten Sprachen werden
  unterstuetzt; unbekannte Sprachen verwenden Englisch.
- Die Wegsuche ignoriert Geländekollisionen fuer Rohre.
- Die endgueltige Baupruefung akzeptiert vergrabene Rohrabschnitte.
- Unterirdische Abschnitte gelten als vom umgebenden Gelaende getragen.
- Rohre, Rohrverbinder und Rohr-Ausgleicher koennen bis zu sechs
  Hoehenstufen nach unten gebaut werden.
- Beim Absenken eines Rohres zeigt eine kleine Tafel am Mauszeiger die
  aktuelle relative Tiefe an, zum Beispiel "Tiefe: -4". Auch dieser Text
  folgt der gewaehlten Spielsprache.
- Beim Auswaehlen eines Fluessigkeitsrohres erscheint zusaetzlich ein
  Sichttiefenfenster mit "Alle" und -1 bis -6. Eine ausgewaehlte Tiefe zeigt
  nur bestehende unterirdische Rohre, Verbinder und Ausgleicher dieser Ebene;
  die anderen Ebenen werden vollstaendig ausgeblendet. Die aktuelle
  Rohrvorschau und ihre mit Hoch/Runter gewaehlte Bauhoehe bleiben davon
  unberuehrt. Das Fenster kann fuer die aktuelle Bausitzung geschlossen
  werden.
- Die Gelaendekollision wird fuer die Wegsuche nur gelockert, wenn Start oder
  Ziel des geplanten Rohrabschnitts bewusst unter dem lokalen Gelaende liegt.
  Ein normal auf der Oberflaeche gebautes Rohr taucht deshalb nicht mehr
  automatisch durch einen dazwischenliegenden Huegel ab.
- Auch einzeln platzierte Rohrverbinder und Rohr-Ausgleicher erhalten die
  Hoehensteuerung und duerfen in das Gelaende eingebettet werden.
- Unterirdische Rohrverbinder duerfen unmittelbar neben einem vorhandenen
  Rohrverbinder automatisch an ein durchlaufendes Rohr angeschlossen werden.
  Das Rohr wird am Anschluss korrekt getrennt und verbunden; die irrefuehrende
  rote Anschluss-Sperre und die unsichtbare Durchleitung entfallen.
- Sehr kurze Rohrabschnitte, die beim automatischen Einsetzen eines kleinen
  Rohrverbinders entstehen, werden ebenfalls durchgehend in der
  Roentgenansicht dargestellt.
- Die Blaupausenvorschau behaelt die ausgewaehlte unterirdische Hoehe bei.
- Nur die tatsaechlich vergrabenen Abschnitte einer Rohr-Blaupause werden als
  farbige Roentgenvorschau durch das Gelaende hindurch angezeigt.
- Bereits fertig gebaute, vergrabene Rohre werden beim Verlegen weiterer
  Rohre ebenfalls durch das Gelaende sichtbar gemacht. Das gilt auch fuer
  vergrabene Rohrverbinder und Rohr-Ausgleicher.
- Das originale Roentgen-Symbol ist als eigener Schalter unten im linken
  Werkzeugbereich verfuegbar. Damit kann das gesamte vergrabene Rohrnetz
  unabhaengig vom aktuell verwendeten Bau-, Abriss- oder Abbauwerkzeug ein-
  und ausgeblendet werden.
- Die Roentgendarstellung ist deckender, damit der Rohrverlauf auch unter
  Gebaeuden klar erkennbar bleibt.
- Fertige unterirdische Rohre verwenden in der Bauansicht automatisch die
  aktuelle Transportfarbe der enthaltenen Fluessigkeit. Wird ein Rohr leer,
  behaelt es die Farbe der zuletzt transportierten Fluessigkeit, bis ein
  anderes Medium eintritt. Nur noch nie verwendete Rohre bleiben cyan.
- Geplante und noch im Bau befindliche Rohre, Verbinder und Ausgleicher
  erscheinen halbtransparent grau. Kleine statische dunkelweisse Striche
  kennzeichnen unfertige Rohrstrecken. Die normalen animierten
  Richtungspfeile bleiben dabei sichtbar, sodass die spaetere Flussrichtung
  schon vor der Fertigstellung erkennbar ist. Die Untergrundansicht wird
  waehrend Bau- und Werkzeugaktionen in kurzen Abstaenden aktualisiert, damit
  automatische Anschluesse ohne die fruehere Wartezeit sichtbar werden.
- Beim einzelnen Platzieren eines Rohrverbinders oder Rohr-Ausgleichers wird
  das vorhandene unterirdische Rohrnetz ebenfalls sichtbar eingeblendet.
- Eigene animierte Richtungspfeile zeigen auf der Roentgenansicht den Verlauf
  auch in Kurven und Gefaellen.
- Die animierten Pfeile eines zusammenhaengenden Rohrabschnitts werden in
  einem gemeinsamen dynamischen Mesh dargestellt. Dadurch muss die bewegte
  Vorschau einer kopierten Anlage nicht mehr fuer jeden Pfeil ein separates
  Unity-Objekt, einen Renderer und einen Animator erzeugen.
- Beim Abrisswerkzeug werden vergrabene Rohre als gelb-orange Warnansicht
  durch das Gelaende sichtbar und beim Verlassen wieder ausgeblendet. Auch
  die unterirdischen Verbinder und Ausgleicher werden dabei hervorgehoben.
- Im Abrisswerkzeug stehen die Tiefensegmente "Alle" und -1 bis -6 zur
  Verfuegung. Der aktive Knopf ist deutlich hervorgehoben. Eine gewaehlte
  Tiefe macht nur Fluessigkeitsrohre, Verbinder und Ausgleicher dieser Ebene
  auswaehlbar. Passende Abschnitte erscheinen hellblau, sodass die rote
  originale Abrisskontur des ausgewaehlten Objekts klar sichtbar bleibt;
  alle anderen Untergrundtiefen werden vollstaendig ausgeblendet. Der
  Filter begrenzt sowohl einzelne Klicks als auch den roten Abrissrahmen auf
  exakte Teilstrecken, ohne automatisch ein ganzes Rohrnetz zu entfernen.
- Das direkt mit der Maus anvisierte Rohrsegment hebt sich dabei mit einem
  abwechselnd dunkelrot-orangen Koerpermuster von allen anderen sichtbaren
  Rohren ab. Beide Farben umschliessen den Rohrkoerper vollstaendig; helle
  animierte Richtungspfeile bleiben darueber gut erkennbar.
- Enthaelt ein Abrissrahmen gleichzeitig andere Gebaeude und unterirdische
  Fluessigkeitsrohre, erscheint nahe dem Mauszeiger eine Auswahl: "Alles
  abreissen" entfernt die gesamte Auswahl, "Rohre ignorieren" laesst nur die
  unterirdischen Rohre und Rohrverteiler stehen. Ein einzeln angeklicktes
  unterirdisches Rohr wird ohne Nachfrage abgerissen. Solange die Auswahl
  geoeffnet ist, bleibt die Simulation pausiert. Der rote Rahmen und seine
  enthaltenen Ziele werden eingefroren und koennen durch Mausbewegungen nicht
  mehr veraendert werden. Die Abfrage greift auch beim Sofortabriss und bei
  unfertigen Bauteilen in einem neuen Spielstand.
- Enthaelt ein Kopierrahmen gleichzeitig andere Gebaeude und unterirdische
  Fluessigkeitsrohre, wird die genaue Auswahl eingefroren und vor dem Erzeugen
  der Kopierblaupause gefragt, ob die Rohre mitkopiert werden sollen. "Ja"
  uebernimmt alles; "Nein" entfernt nur die unterirdischen Rohre, Verbinder
  und Ausgleicher aus der Kopie. Eine bewusst allein ausgewaehlte
  unterirdische Rohrstrecke wird ohne Nachfrage kopiert. Die Abfrage gilt nur
  fuer das Kopierwerkzeug, nicht fuer Ausschneiden, und folgt ebenfalls der
  gewaehlten Spielsprache.
- In den Sicherheitsfragen fuer Kopieren und Abriss wird der entscheidende
  Begriff "unterirdische Fluessigkeitsrohre" fett, in 120 Prozent
  Schriftgroesse und in einem gut lesbaren Goldton hervorgehoben. Der zweite
  Satz bezieht sich mit "diese Rohre" eindeutig auf genau diese Bauteile.
- Beim Abbauwerkzeug werden unterirdische Fluessigkeitsrohre orange sichtbar.
  Liegt der Geländecursor direkt ueber einem solchen Rohr oder kreuzt der rote
  Abbau-Auswahlrahmen ein Rohr, erscheint zusaetzlich eine Warnung am
  Mauszeiger.
- Beim Bearbeiten des gelben Abbaugebiets eines Minenturms werden
  unterirdische Fluessigkeitsrohre ebenfalls orange sichtbar. Sobald die
  gesamte aktuell bearbeitete Flaeche ein solches Rohr kreuzt, erscheint am
  Mauszeiger die Warnung "Achtung: Das Abbaugebiet kreuzt unterirdische
  Fluessigkeitsrohre!". Logistikzonen bleiben davon unberuehrt.
- Bei grossen 2x2-Rohr-Ausgleichern wird fuer die Untergrundansicht nur das
  sichtbare Modell kopiert; Kamera und sonstige Funktionskomponenten bleiben
  unberuehrt.
- Oberirdische Rohrabschnitte benoetigen weiterhin normale Pfeiler.
- Foerderbaender, Schuettgutbaender, Schmelzkanäle und andere Gebaeude
  bleiben unveraendert.

Installation
------------
Den Ordner "PipeTerrainIgnoreMod" nach
%APPDATA%\Captain of Industry\Mods\
kopieren und den Mod im Menue "DLC & Mods" aktivieren.

Kompatibilitaet
---------------
Erstellt und geprueft gegen Captain of Industry 0.8.7 (Build 613).
Der Mod fuegt keine eigenen Speicherdaten hinzu und kann daher zu einem
bestehenden Spielstand hinzugefuegt oder wieder entfernt werden.

Hinweis
-------
Vor dem ersten Einsatz empfiehlt sich ein separater Testspielstand.
