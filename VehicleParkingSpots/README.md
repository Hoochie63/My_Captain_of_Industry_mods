# Vehicle Parking Spots

Version 0.9.0. Compatible and verified with Captain of Industry 0.8.6c.

Five buildable, non-blocking parking areas for idle vehicles:

- Light vehicles: 3 x 5
- Trucks: 4 x 6
- Large trucks: 5 x 8
- Excavators: 5 x 7
- Heavy machinery: 6 x 9

Parking areas are dedicated saved entities with no construction cost, power demand, production logic, or physical floor. Placement and removal finish immediately. Their visible marker consists only of four small orange traffic cones and a colored exit arrow; there is no raised fence or concrete object.

After an idle grace period, a compatible idle vehicle is routed to the center of the nearest free space through the game's native vehicle goal and navigation job. A new real job always takes precedence and releases the reservation.

The inspector supports public parking, parking restricted to the space's vehicle-size category, and map selection of one exact vehicle. The selected vehicle remains available to the normal job system; parking never uses the vehicle's operational `AssignedTo` relationship. Reservation, occupancy, access mode, and vehicle preference are stored in the save.

Vehicles navigate to the exact center of the bay using the native position-and-direction goal and stop facing the exit. Parking and exit occupancy are checked as rotated rectangles. Removing a bay cancels its pending parking navigation, clears its reservation and does not leave a stale vehicle assignment.

## Save compatibility

The mod can be added to an existing save. It must not be removed from a save that contains its parking entities. Back up saves before updating mods.

## Copyright

Copyright © 2026 Sirael  
Contact: alena.verabei@protonmail.com  
IBAN: BE94967304158014  
[https://buymeacoffee.com/sirael](https://buymeacoffee.com/sirael)  
All rights reserved.
