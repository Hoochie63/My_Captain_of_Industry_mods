Truck Parking 1.0.1
===================

Kompatibel mit Captain of Industry v0.8.6a.

Der Mod fügt im Fahrzeug-Menü einen drehbaren Parkplatz mit vier Stellplätzen
hinzu.

Verhalten
---------

Ein Truck wird erst zum Parkplatz geschickt, wenn er:

  - leer ist,
  - keine echte Aufgabe besitzt,
  - keinem Gebäude, Minenturm oder Forstturm fest zugewiesen ist,
  - nicht tanken, verschrottet oder ersetzt werden soll,
  - einige Sekunden frei im Leerlauf stand.

Der nächstgelegene freie Stellplatz wird vor der Abfahrt reserviert. Andere
Trucks können denselben Slot dadurch nicht gleichzeitig erhalten. Ein neuer
Transportauftrag hat immer Vorrang: Die Parkfahrt ist absichtlich nur ein
unterbrechbarer Niedrigprioritätsjob.

Logistikzonen
-------------

Der Parkplatz übernimmt die Logistikzone an seiner Position. Trucks verwenden
nur Parkplätze, deren Zone zu ihrer eigenen Zone passt.

Aufstellung
-----------

Der kleine gelbe Marker ist das eigentliche Gebäude. Die vier markierten
Stellflächen liegen in Pfeilrichtung davor. Für zuverlässige Wegfindung sollte
die gesamte sichtbare Fläche eben, frei und über Land erreichbar sein.

Große Haul Trucks werden unterstützt. Sind alle vier Slots belegt oder
reserviert, bleiben weitere freie Trucks an ihrer bisherigen Position.

Spielstände
-----------

Der Mod kann einem bestehenden Spielstand hinzugefügt werden. Reservierungen
werden nach jedem Laden automatisch neu aufgebaut. Sobald ein Parkplatz im
Spielstand gespeichert wurde, darf der Mod aus diesem Spielstand nicht mehr
entfernt werden.

Lizenzhinweis
-------------

Das Paket enthält keinen Quellcode und keine veränderten Assets von Captain of
Industry. Es verwendet die öffentliche Mod-API und referenziert zur Laufzeit
ein Material und ein Icon aus der vorhandenen Spielinstallation.

Änderungen in 1.0.1
-------------------

  - Behebt den Speicherfehler "Failed to create generic serializer for
    'TruckParkingManager'".
  - Laufzeit-Reservierungen werden nicht mehr in den Spielstand-Eventgraphen
    aufgenommen und nach dem Laden wie vorgesehen neu aufgebaut.
