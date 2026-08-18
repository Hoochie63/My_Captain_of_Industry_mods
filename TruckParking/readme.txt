Truck Parking 1.3.3
===================

Kompatibel mit Captain of Industry v0.8.6c bis v0.8.7.

English documentation: readme.en.txt

Sprache
-------

MultiLangLib 0.1.0 oder neuer wird benötigt und muss zusammen mit diesem Mod
aktiviert sein. Externe JSON-Kataloge decken alle 21 Sprachen der aktuellen
Spielversion ab: Deutsch, Englisch, Französisch, Spanisch, Portugiesisch
(Brasilien), Ukrainisch, Russisch, Katalanisch, Chinesisch (vereinfacht und
traditionell), Tschechisch, Niederländisch, Estnisch, Ungarisch, Italienisch,
Japanisch, Koreanisch, Norwegisch, Polnisch, Schwedisch und Türkisch.
Englisch bleibt der Fallback für unvollständige Community-Übersetzungen.

Eigener UI-Reiter und 48 Varianten
----------------------------------

Der Mod fügt direkt neben dem Reiter „Vehicles“ einen eigenen Reiter
„Parkplätze“ hinzu. Dieser ist in „Transparent“ und „Gepflastert“
unterteilt. Jeder Unterreiter zeigt sechs Elemente für 1, 2, 3, 4, 6 oder
12 Stellplätze. Wie bei Förderbändern und anderen mehrstufigen Bauteilen
öffnet jedes Element links einen nativen T1-bis-T4-Selektor:

  - Klein (T1):   6 x 3 Tiles pro Stellplatz
  - Mittel (T2):  7 x 4 Tiles pro Stellplatz
  - Groß (T3):    8 x 5 Tiles pro Stellplatz
  - Super (T4):  10 x 7 Tiles pro Stellplatz

Jede Größenklasse enthält Parkflächen mit 1, 2, 3, 4, 6 oder 12
Stellplätzen, jeweils transparent und gepflastert. Damit stehen insgesamt
48 sichtbare Varianten bereit. Zwischen benachbarten Stellplätzen bleibt
immer eine vollständige Tile-Reihe für Mauern oder schmale Trenner frei.

Die Fahrzeugabmessungen werden geprüft. Ein Fahrzeug wird nur einem Slot
zugewiesen, in den sein geladener Prototyp tatsächlich passt. Die
Super-Variante ist ausdrücklich für übergroße Fahrzeuge aus anderen Mods
vorgesehen.

Varianten und Wartung
---------------------

Transparent:

  - kein eigener Boden; das vorhandene Gelände bleibt sichtbar,
  - normale oder durch andere Mods bereitgestellte Tiles können darunter
    verlegt werden,
  - ein geparktes Fahrzeug erhält zusätzlich 5 Prozentpunkte
    Wartungsvorteil.

Beispiele: blankes Gelände 100 % -> 95 %, normaler Beton 80 % -> 75 %.

Gepflastert:

  - besitzt eine sichtbare, befahrbare Plattenfläche,
  - vorhandene Tiles im Baufeld werden beim Bau entfernt und erstattet,
  - neue Tiles können unter der Parkfläche nicht ausgewiesen werden,
  - ein geparktes Fahrzeug arbeitet mit insgesamt 50 %
    Wartungsmultiplikator,
  - die Baukosten liegen rund 10 % unter der manuellen Belegung derselben
    Fläche mit Betonplatten.

Betonplatten für 1 / 2 / 3 / 4 / 6 / 12 Slots:

  - Klein:   16 / 32 / 48 / 64 / 97 / 194
  - Mittel:  25 / 50 / 75 / 100 / 151 / 302
  - Groß:    36 / 72 / 108 / 144 / 216 / 432
  - Super:   63 / 126 / 189 / 252 / 378 / 756

Fahrzeugfilter (1:n)
---------------------

Im Inspektor jeder neuen Parkfläche befindet sich ein Filter-Symbol. Dort
können beliebig viele Fahrzeugtypen ausgewählt werden. Eine leere Auswahl
erlaubt alle unterstützten Bodenfahrzeuge, schließt erkannte Helikopter,
Zeppeline und andere Luftfahrzeuge jedoch standardmäßig aus. Eine ausdrückliche
Auswahl ist vollständig maßgeblich, sodass Luftfahrzeuge bewusst freigeschaltet
werden können. Die Liste wird dynamisch aus allen geladenen Prototypen erzeugt,
deren Laufzeittyp von Vehicle erbt. Dadurch werden Basisfahrzeuge und
kompatible Fahrzeug-Mods gemeinsam unterstützt.

Neben dem Filter befindet sich eine Reset-Schaltfläche. Sie leert die gesamte
Auswahl und entfernt auch gespeicherte IDs von inzwischen deinstallierten
Fahrzeug-Mods. Filter werden gespeichert und beim Kopieren einer Parkfläche
übernommen.

Gebäudezuordnung
----------------

Die Plus/Minus-Schaltfläche im Parkplatz-Inspektor startet die native
Kartenauswahl. Gewählt werden können alle statischen Gebäude, welche die
offizielle Fahrzeugzuweisung des Spiels unterstützen – einschließlich
kompatibler Mod-Gebäude. Danach zeigt der Inspektor Name und Symbol des
aktuellen Gebäudes. Das Symbol zentriert die Kamera auf dem Ziel; die
Abbrechen-Schaltfläche löscht die Verbindung.

Eine verknüpfte Parkfläche akzeptiert ausschließlich Fahrzeuge, deren
AssignedTo-Ziel exakt dieses Gebäude ist. Mehrere Parkflächen dürfen dasselbe
Gebäude bedienen. Für passende Fahrzeuge kann die explizite Verbindung die
lokale Logistikzone überbrücken; nach dem Löschen gilt wieder die normale
Zonenprüfung. Sind alle passenden Slots belegt oder unerreichbar, bleibt das
native Warteverhalten des Gebäudes erhalten. Echte Arbeits-, Tank-, Transfer-,
Ersatz- und Verschrottungsaufträge haben immer Vorrang vor dem Parken.

Die Verbindung wird im Spielstand gespeichert und beim Kopieren der
Parkflächeneinstellungen übernommen. Blaupausen ordnen ein gemeinsam
enthaltenes Ziel korrekt neu zu und verwerfen externe Ziele, damit eine
eingefügte Blaupause nicht unbemerkt ein fremdes Gebäude verknüpft. Wird das
Ziel entfernt, zerstört oder nicht mehr unterstützt, verhält sich die Fläche
sicher wie unverknüpft; der verwaiste Eintrag kann im Inspektor gelöscht
werden.

Unterstützte Fahrzeuge und Verhalten
------------------------------------

Der Manager verarbeitet alle vom Spiel in IVehiclesManager.AllVehicles
registrierten Fahrzeuge. Dazu gehören Trucks, Bagger, Baumernter,
Baumpflanzer und weitere kompatible Modfahrzeuge. Helikopter und Zeppeline
werden automatisch erkannt und meiden ungefilterte Parkplätze. Über die
ausdrückliche Auswahl ihres Prototyps im Filter bleiben sie als Opt-in möglich.

Ein Fahrzeug wird erst zum Parkplatz geschickt, wenn es:

  - gespawnt, aktiviert und wirklich im Leerlauf ist,
  - keine echte Aufgabe besitzt; nur die Leerlauf-Rückfahrt zum Besitzer darf
    ersetzt werden,
  - für normale Parkflächen unzugewiesen ist, exakt dem mit der Parkfläche
    verknüpften Gebäude gehört oder für die unten beschriebene
    Tankstellen-Ausnahme geeignet ist,
  - nicht tanken, verschrottet oder ersetzt werden soll,
  - einige Prüfintervalle frei im Leerlauf stand,
  - vom Filter zugelassen wird und in die Größenklasse passt.

Trucks und Bagger müssen außerdem leer sein. Bei Baumerntern werden auch
Ladung, Baumziel und wartender Truck geprüft; Baumpflanzer dürfen keinen
aktiven Pflanzvorgang besitzen.

Ein einer Tankstelle zugewiesener Tankwagen darf einen externen Stellplatz
nutzen, wenn die Tankstelle ihn vollständig mit ihrem konfigurierten
Treibstoff beladen hat. Ein leerer zugewiesener Tankwagen darf nur parken,
solange seine Station deaktiviert ist oder keinen Treibstoff lagert.
Teilbeladene Tankwagen bleiben stets für den nativen Nachladezyklus reserviert.
Ihre Rückfahrt zur Station wird niemals zugunsten des Parkplatzes abgebrochen.
Der Mod überträgt oder erzeugt keinen Treibstoff. Echte Tank-, Transfer- und
Lieferaufträge haben immer Vorrang.

Der nächstgelegene freie Stellplatz wird vor der Abfahrt reserviert. Eine neue
echte Aufgabe hat immer Vorrang: Die Parkfahrt ist nur ein unterbrechbarer
Niedrigprioritätsjob. Unverknüpfte Parkflächen beachten weiterhin die
Logistikzone an ihrer Position; eine explizite Gebäudeverbindung darf sie nur
für Fahrzeuge des passenden Zielgebäudes überbrücken. Damit nach dem Laden
großer Spielstände keine Pathfinding-Spitze entsteht, beginnen pro
Manager-Prüfung höchstens vier neue Parkfahrten; weitere bereite Fahrzeuge
folgen in den nächsten Prüfungen.

Aufstellung
-----------

Der Bau- und Drehpunkt bleibt als unsichtbares, befahrbares und
oberflächenfreies Tile in der Mitte der Fläche erhalten. Einen gesonderten
gelben Mittelpunkt gibt es nicht mehr. Die markierten Slots liegen in Richtung
ihrer Pfeile davor. Breite, flache Auswahl-Collider über den Markierungen
machen geplante und fertige Flächen leichter anklickbar, ohne die
Simulationsbelegung zu verändern. Für zuverlässige Wegfindung sollte die
sichtbare Fläche eben, frei und für den jeweiligen Fahrzeugtyp erreichbar sein.

Nach erfolgreicher Parkfahrt rastet das Fahrzeug längs und quer exakt mittig im
markierten Quader ein. Zwei Richtungsschaltflächen im Parkplatz-Inspektor
wählen die Ausrichtung mit dem Pfeil oder exakt 180 Grad entgegen dem Pfeil.
Ein erneuter Klick auf die aktive Schaltfläche gibt die Richtung wieder frei;
die exakte Zentrierung bleibt immer aktiv. Der gewählte Zustand wird
gespeichert und beim Kopieren der Parkfläche übernommen.

Spielstände und Upgrade
-----------------------

Die zwölf Parkflächen aus der Vorgängerversion behalten ihre IDs und ihre exakte
8-x-5-Geometrie. Sie erscheinen nach dem Upgrade als Größe Groß (T3), sodass
bestehende Gebäude und Filter ohne Migration weitergeladen werden. Die alte
Vier-Slot-Parkfläche bleibt als versteckte Legacy-Variante
ladefähig.
Diese versteckte Legacy-Variante besitzt keinen Parkplatz-Inspektor; bei ihr
bleibt die Fahrzeugrotation ausgeschaltet. Ein Downgrade auf eine ältere
Mod-Version wird nicht unterstützt, nachdem ein Spielstand mit der aktuelleren
Version erneut gespeichert wurde.

Reservierungen werden nach jedem Laden automatisch neu aufgebaut. Sobald eine
Parkfläche gespeichert wurde, darf der Mod aus diesem Spielstand nicht mehr
entfernt werden.

Änderungen in 1.3.3
-------------------

  - Abbruch nativer Tankstellen-Rückfahrten durch die Gebäudezuordnung
    verhindert.
  - Teilbeladene Tankwagen bleiben für den nativen Nachladezyklus reserviert;
    leere Tankwagen parken nur bei nicht verfügbarer Station.
  - Voll beladene Tankwagen mit passendem Treibstoff dürfen weiterhin extern
    parken; echte wartende Jobs bleiben vollständig erhalten.
  - Eigener selektiv unterbrechbarer Parkjob ergänzt, damit echte Aufträge
    sofort nachrücken, ohne andere Einträge der Fahrzeugwarteschlange zu löschen.
  - Kompatibilitätsbereich bis Captain of Industry 0.8.7 erweitert.
  - Bestehende Spielstände und Parkflächen bleiben aufwärtskompatibel; nach
    erneutem Speichern wird ein Downgrade auf ältere Mod-Versionen nicht
    unterstützt.

Änderungen in 1.3.2
-------------------

  - Rotationsabhängigen Versatz des exakten Stellplatzmittelpunkts behoben.
  - Fahrzeuge rasten jetzt auch auf um 90, 180 oder 270 Grad gedrehten
    Parkflächen exakt auf der gelben Mittellinie ein.
  - Spielstandformat und gespeicherte Parkflächeneinstellungen bleiben
    unverändert kompatibel.

Änderungen in 1.3.1
-------------------

  - Gesonderten gelben Mittelpunkt entfernt; der unsichtbare, befahrbare
    Platzierungs-Pivot bleibt erhalten.
  - Getrennte Schaltflächen für Pfeilrichtung und die um 180 Grad gedrehte
    Gegenrichtung ergänzt.
  - Beide Richtungen bleiben optional und schließen sich gegenseitig aus;
    sind beide aus, gilt weiterhin die bisherige freie Standardausrichtung.
  - Gegenrichtung speicher- und kopierbar ergänzt, ohne das positionale
    Saveformat zu ändern; ältere aktivierte Einstellungen bleiben Pfeilrichtung.

Änderungen in 1.3.0
-------------------

  - Optionale Verbindung von Parkflächen mit Zuweisungsgebäuden über eine
    native Kartenauswahl ergänzt.
  - Live-Anzeige mit Name, Symbol, Kamera-Sprung sowie Ändern- und
    Löschen-Aktion ergänzt.
  - Mehrere Parkflächen pro Gebäude erlaubt und Fahrzeuge exakt nach ihrem
    AssignedTo-Ziel abgeglichen.
  - Natives Warten bei vollen Flächen bewahrt und alle echten Aufgaben über
    das Parken priorisiert.
  - Verbindungen speicher- und kopierbar gemacht, mit Blaupausen-sicherer
    Neuzuordnung und sicherem Fallback für fehlende oder zerstörte Ziele.
  - Logistikzonen nur für das exakt verknüpfte Zielgebäude überbrückt.
  - Neue Bedien- und Fehlertexte in alle MultiLangLib-Kataloge aufgenommen.

Änderungen in 1.2.0
-------------------

  - Alle sichtbaren Texte auf MultiLangLib-JSON-Kataloge umgestellt und alle
    aktuell vom Spiel gelieferten Sprachen ergänzt.
  - Externes Parken beladener Tankstellenfahrzeuge ohne Gratistreibstoff
    ergänzt.
  - Erkannte Luftfahrzeuge standardmäßig ausgeschlossen; ausdrückliches
    Opt-in bleibt möglich.
  - Auswahl im Planungsmodus verbessert und den kleineren Pivot befahrbar
    gemacht.
  - Massenhafte Parkzuweisungen über mehrere Prüfintervalle verteilt, um die
    Pathfinding-Last zu glätten.
  - Drei eigene, hochkontrastreiche Parkplatz-Icons im CoI-Toolbar-Stil
    ergänzt.
  - Namen zeigen die Stellplatzzahl nun vor der Größenklasse.
  - Deterministischen Build, Release-Paketprüfung und Regressionstests ergänzt.

Änderungen in 1.1.4
-------------------

  - Stellplatzquader am Tile-Raster zentriert und die freie Reihe zwischen
    benachbarten Stellplätzen auf exakt ein Tile korrigiert.
  - Fahrzeuge rasten nach erfolgreicher Parkfahrt längs und quer exakt in der
    Quader-Mitte ein.
  - Optionale Pfeilausrichtung pro Parkplatz ergänzt; standardmäßig ist die
    Fahrzeugrotation ausgeschaltet.

Änderungen in 1.1.2
-------------------

  - Parkflächen wie andere mehrstufige UI-Elemente gruppiert.
  - Eigene Unterreiter für transparente und gepflasterte Varianten ergänzt.
  - Je Slotanzahl einen nativen T1-bis-T4-Selektor ergänzt.
  - Bestehende Gebäude- und Toolbar-IDs für Spielstände beibehalten.

Änderungen in 1.1.0
-------------------

  - Eigenen Parking-Reiter direkt neben Vehicles ergänzt.
  - Größenreiter Small, Medium, Large und Super ergänzt.
  - Variantenbestand von 12 auf 48 sichtbare Parkflächen erweitert.
  - Manager und 1:n-Filter von Trucks auf alle Vehicle-Unterklassen erweitert.
  - Bagger und weitere Arbeitsfahrzeuge mit sicheren Idle-Prüfungen ergänzt.
  - Vollständiges Leeren des Filters inklusive verwaister Mod-IDs ergänzt.
  - Größenprüfung für Basis- und Modfahrzeuge ergänzt.
  - Doppelte Manager-Freigabe beim Beenden behoben.
  - Abbruch veralteter Niedrigprioritäts-Parkjobs korrigiert.
  - Vollständige Ingame-Texte auf Deutsch, Englisch, Ukrainisch, Russisch,
    Französisch, Spanisch und Portugiesisch ergänzt.

Lizenzhinweis
-------------

Die drei Parkplatz-Icons sind eigens erzeugte Assets in einem Unity-
AssetBundle. Es werden keine Grafiken aus Captain of Industry weitergegeben.
Herkunft und Lizenz der offenen AssetBundle-Container-Vorlage stehen in
THIRD_PARTY_NOTICES.md und licenses/COI-Open-MetallurgyPlus.txt.
