from Mafi import Duration, Quantity
from Mafi.Base import Assets, Ids
from CustomAssets import add_loose_product_material, add_texture, build_product_loose, build_recipe, Product

Research_Plasma_Inceneritor = build_research(
    "Research_Plasma_Inceneritor",
    "Plasma Incinerator",
    "This research enables the extraction of plasma from the nuclear fusion reactor, and through purification cycles, it produces plasma that can be used in non-nuclear industrial systems. It also unlocks Incinerator II, which allows plasma to be used as fuel for burning waste.",
    costs = 20892,
    position = (170, 50),
    icon = "Assets/Products/Plasma.png"
)

Incineration_plant_T2_machine = build_machine(
    machineId            = "Incineration_plant_T2",
    source               = "IncinerationPlant",
    name = "Incineration Plant II",
    description = "It incinerates waste with far greater efficiency than a traditional burner and incinerator. The process has a positive energy balance and produces steam. Thanks to the plasma, no pollution is generated, only slag from inorganic waste.",
    ports                = [
        Port(name="Z", type="output", shape="IoPortShape_LooseMaterialConveyor", position=(8, 0, 0), direction="-Y")
    ],
    research = Research_Plasma_Inceneritor
)

plasma_trash_recipe = build_recipe(
    "Recipe_plasma_trash",
    "Plasma Waste Burning",
    "It uses plasma to incinerate waste, converting inorganic waste into slag and organic waste into combustible gas. Water is used to cool the system, producing super-pressurized steam.",
    "Incineration_plant_T2",
    research = Research_Plasma_Inceneritor,
    duration = Duration.FromSec(20),
    ingredients = [
        Product("Product_plasma", Quantity(2)),
        Product("Product_Waste", Quantity(48)),
        Product("Product_Water", Quantity(6))
    ],
    products = [
        Product("Product_Slag", Quantity(12)),
        Product("Product_SteamSp", Quantity(6)),
        Product("Product_FuelGas", Quantity(12))
    ]
)

plasma_trash_compact_recipe = build_recipe(
    "Recipe_plasma_trash_compact",
    "Plasma Waste Pressed Burning",
    "It uses plasma to incinerate waste pressed, converting inorganic waste into slag and organic waste into combustible gas. Water is used to cool the system, producing super-pressurized steam.",
    "Incineration_plant_T2",
    research = Research_Plasma_Inceneritor,
    duration = Duration.FromSec(20),
    ingredients = [
        Product("Product_plasma", Quantity(2)),
        Product("Product_WastePressed", Quantity(16)),
        Product("Product_Water", Quantity(6))
    ],
    products = [
        Product("Product_Slag", Quantity(12)),
        Product("Product_SteamSp", Quantity(6)),
        Product("Product_FuelGas", Quantity(12))
    ]
)

unlcok_dirtyplasma = add_unlock_product(
    Research_Plasma_Inceneritor,
    "Product_plasmadirty"
)

unlcok_plasma_refined = add_unlock_product(
    Research_Plasma_Inceneritor,
    "Product_plasmarefined"
)

unlcok_plasma_pure = add_unlock_product(
    Research_Plasma_Inceneritor,
    "Product_plasma"
)

unlock_plasma_refined_recipe = add_unlock_recipe(
    Research_Plasma_Inceneritor,
    "UraniumEnrichmentPlant",
    "Recipe_refinedplasma"
)

unlock_plasma_refined_recipe = add_unlock_recipe(
    Research_Plasma_Inceneritor,
    "UraniumEnrichmentPlant",
    "Recipe_plasma"
)

