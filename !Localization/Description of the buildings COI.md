1. There's no extended (custom) description for buildings, or one generic description is used for multiple levels.
   For example, "SettlingTank__name" is present, but "SettlingTank__desc" is missing:

| Structure (`__name`)                     | Expected description key                 | Name in the game (EN)            |
|------------------------------------------|------------------------------------------|----------------------------------|
| `AssemblyMachine__name`                  | `AssemblyElectrifiedT2__desc`            | Assembly III                     |
| `AssemblyMachine__name`                  | `AssemblyRoboticT1__desc`                | Assembly V                       |
| `BarrierCorner__name`                    | `BarrierCorner__desc`                    | Barrier (corner)                 |
| `BarrierCross__name`                     | `BarrierCross__desc`                     | Barrier (cross)                  |
| `BarrierEnd__name`                       | `BarrierEnd__desc`                       | Barrier (ending)                 |
| `BarrierTee__name`                       | `BarrierTee__desc`                       | Barrier (tee)                    |
| `BasicServerRack__name`                  | `BasicServerRack__desc`                  | Basic rack                       |
| `BauxiteMine__name`                      | `BauxiteMine__desc`                      | Bauxite quarry                   |
| `CaptainOfficeT1__name`                  | `CaptainOfficeT1__desc`                  | Captain's office I               |
| `CaptainOfficeT2__name`                  | `CaptainOfficeT2__desc`                  | Captain's office II              |
| `CargoDepotT1__name`                     | `CargoDepotT1__desc`                     | Cargo depot (2)                  |
| `CargoDepotT2__name`                     | `CargoDepotT2__desc`                     | Cargo depot (4)                  |
| `CargoDepotT3__name`                     | `CargoDepotT3__desc`                     | Cargo depot (6)                  |
| `CargoDepotT4__name`                     | `CargoDepotT4__desc`                     | Cargo depot (8)                  |
| `CargoShipT2__name`                      | `CargoShipT2__desc`                      | Cargo Ship (4 modules)           |
| `CargoShipT3__name`                      | `CargoShipT3__desc`                      | Cargo Ship (6 modules)           |
| `CargoShipT4__name`                      | `CargoShipT4__desc`                      | Cargo Ship (8 modules)           |
| `CasterCooledT2__name`                   | `CasterCooledT2__desc`                   | Cooled caster II                 |
| `CasterT2__name`                         | `CasterT2__desc`                         | Metal caster II                  |
| `ChemicalPlant2__name`                   | `ChemicalPlant2__desc`                   | Chemical plant II                |
| `CoalMine__name`                         | `CoalMine__desc`                         | Coal mine                        |
| `ConcreteMixerT3__name`                  | `ConcreteMixerT3__desc`                  | Concrete Mixer III               |
| `CoolingTowerT2__name`                   | `CoolingTowerT2__desc`                   | Cooling tower (large)            |
| `DistillationTowerT3__name`              | `DistillationTowerT3__desc`              | Distillation (stage III)         |
| `ElectrolyzerT2__name`                   | `ElectrolyzerT2__desc`                   | Electrolyzer II                  |
| `FarmT4__name`                           | `FarmT4__desc`                           | Greenhouse II                    |
| `FlatConveyorT1__name`                   | `FlatConveyorT1__desc`                   | Flat conveyor                    |
| `GoldFurnace__name`                      | `GoldFurnace__desc`                      | Gold furnace                     |
| `HousingT3__name`                        | `HousingT3__desc`                        | Housing III                      |
| `HousingT4__name`                        | `HousingT4__desc`                        | Housing IV                       |
| `HydrogenReformer__name`                 | `HydrogenReformer__desc`                 | Hydrogen reformer                |
| `IndustrialMixerT2__name`                | `IndustrialMixerT2__desc`                | Mixer II                         |
| `LimestoneMine__name`                    | `LimestoneMine__desc`                    | Limestone quarry                 |
| `LooseMaterialConveyor__name`            | `LooseMaterialConveyor__desc`            | U-shape conveyor                 |
| `MaintenanceDepotT0__name`               | `MaintenanceDepotT0__desc`               | Maintenance depot (basic)        |
| `MaintenanceDepotT2__name`               | `MaintenanceDepotT2__desc`               | Maintenance II depot             |
| `MaintenanceDepotT3__name`               | `MaintenanceDepotT3__desc`               | Maintenance III depot            |
| `MicrochipMachineT2__name`               | `MicrochipMachineT2__desc`               | Microchip machine II             |
| `OreSortingPlantT2__name`                | `OreSortingPlantT2__desc`                | Ore sorting plant (large)        |
| `OxygenFurnaceT2__name`                  | `OxygenFurnaceT2__desc`                  | Oxygen furnace II                |
| `PolymerizationPlant__name`              | `PolymerizationPlant__desc`              | Polymerization plant             |
| `QuartzMine__name`                       | `QuartzMine__desc`                       | Quartz mine                      |
| `RetainingWallCorner__name`              | `RetainingWallCorner__desc`              | Retaining wall (corner)          |
| `RetainingWallCross__name`               | `RetainingWallCross__desc`               | Retaining wall (cross)           |
| `RetainingWallStraight4__name`           | `RetainingWallStraight4__desc`           | Retaining wall (long)            |
| `RetainingWallTee__name`                 | `RetainingWallTee__desc`                 | Retaining wall (tee)             |
| `RockMine__name`                         | `RockMine__desc`                         | Rock mine                        |
| `RotaryKilnGas__name`                    | `RotaryKilnGas__desc`                    | Rotary Kiln (gas)                |
| `SettlementFountain__name`               | `SettlementFountain__desc`               | Square with fountain             |
| `SettlementPillar__name`                 | `SettlementPillar__desc`                 | Square with column               |
| `SettlementSmall1__name`                 | `SettlementSmall1__desc`                 | Settlement                       |
| `SettlementSquare1__name`                | `SettlementSquare1__desc`                | Square (light)                   |
| `SettlementSquare2__name`                | `SettlementSquare2__desc`                | Square (dark)                    |
| `SettlingTank__name`                     | `SettlingTank__desc`                     | Settling tank                    |
| `Shaft__name`                            | `Shaft__desc`                            | Shaft                            |
| `SmokeStackLarge__name`                  | `SmokeStackLarge__desc`                  | Smoke stack (large)              |
| `StorageFluidT2__name`                   | `StorageFluidT2__desc`                   | Fluid storage II                 |
| `StorageFluidT3__name`                   | `StorageFluidT3__desc`                   | Fluid storage III                |
| `StorageFluidT4__name`                   | `StorageFluidT4__desc`                   | Fluid storage IV                 |
| `StorageFluid__name`                     | `StorageFluid__desc`                     | Fluid storage                    |
| `StorageLooseT2__name`                   | `StorageLooseT2__desc`                   | Loose storage II                 |
| `StorageLooseT3__name`                   | `StorageLooseT3__desc`                   | Loose storage III                |
| `StorageLooseT4__name`                   | `StorageLooseT4__desc`                   | Loose storage IV                 |
| `StorageLoose__name`                     | `StorageLoose__desc`                     | Loose storage                    |
| `StorageUnitT2__name`                    | `StorageUnitT2__desc`                    | Unit storage II                  |
| `StorageUnitT3__name`                    | `StorageUnitT3__desc`                    | Unit storage III                 |
| `StorageUnitT4__name`                    | `StorageUnitT4__desc`                    | Unit storage IV                  |
| `StorageUnit__name`                      | `StorageUnit__desc`                      | Unit storage                     |
| `SulfurMine__name`                       | `SulfurMine__desc`                       | Sulfur mine                      |
| `TombOfCaptainsStageFinal__name`         | `TombOfCaptainsStageFinal__desc`         | Tomb of Captains                 |
| `TrainLevelCrossingLargeConnector__name` | `TrainLevelCrossingLargeConnector__desc` | Level crossing connector (large) |
| `TrainLevelCrossingLargeEntrance__name`  | `TrainLevelCrossingLargeEntrance__desc`  | Level crossing entrance (large)  |
| `TurbineHighPressT2__name`               | `TurbineHighPressT2__desc`               | High-pressure turbine II         |
| `TurbineLowPressT2__name`                | `TurbineLowPressT2__desc`                | Low-pressure turbine II          |
| `UraniumMine__name`                      | `UraniumMine__desc`                      | Uranium mine                     |
| `VehicleRamp2__name`                     | `VehicleRamp2__desc`                     | Vehicle ramp (medium)            |
| `VehicleRamp3__name`                     | `VehicleRamp3__desc`                     | Vehicle ramp (large)             |
| `VehiclesDepotT2__name`                  | `VehiclesDepotT2__desc`                  | Vehicles depot II                |
| `VehiclesDepotT3__name`                  | `VehiclesDepotT3__desc`                  | Vehicles depot III               |
| `WaterChiller__name`                     | `WaterChiller__desc`                     | Water chiller                    |
 

