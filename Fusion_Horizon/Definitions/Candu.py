from Mafi import Duration, Quantity
from Mafi.Base import Assets, Ids
from CustomAssets import add_loose_product_material, add_texture, build_product_loose, build_recipe, Product

candu_rod_icon = add_texture("Assets/Products/CANDURod_Icon.png")

heavy_water_icon = add_texture("Assets/Products/HeavyWater.png")

heavy_water_hot_icon = add_texture("Assets/Products/HeavyWaterHigh.png")

candu_rod = add_unit_prefab(
    path   = "Assets/Product_Candu_rod",
    albedo = "Assets/Meshes/candu_rod_texture.png",
    mesh = "Assets/Meshes/Candu_Rod.obj"
)

candu_research = build_research(
    "Research_Candu_Reactor",
    "Nuclear Reactor CANDU",
    "A thermal reactor that sustains a nuclear chain reaction using natural uranium fuel rods. The CANDU nuclear reactor is moderated by heavy water, which requires a heat exchanger to produce steam. Warning: CANDU fuel produces spent fuel, which is radioactive and can harm the public if not stored in a specialized facility.",
    costs = 1692,
    position = (132, 31),
    icon = candu_rod_icon,
    tier = "Product_LabEquipment3"
)

candu_rod_Product = build_product_unit(
    productId = "Product_Candu_rod",
    name      = "CANDU Rod",
    icon      = candu_rod_icon,
    prefab    = candu_rod,
    description = "A CANDU fuel rod consisting of compacted uranium-bearing material enclosed in a sturdy steel cladding, ready for use in heavy-water reactors.",
    isStorable = True,
    research = "Research_Candu_Reactor"
)

heavy_water = build_product_fluid(
    productId = "Product_heavy_water",
    name      = "Heavy Water",
    icon      = heavy_water_icon,
    color = (111, 175, 203),
    transportColor = (28, 84, 104),
    transportAccentColor = (143, 232, 255),
    description = "Heavy Water is a deuterium‑enriched form of water used as a neutron moderator in advanced nuclear reactors, enabling stable and efficient fission processes.",
    isStorable = True
)

heavy_water_hot = build_product_fluid(
    productId = "Product_heavy_water_high",
    name      = "Heavy Water (High)",
    icon      = heavy_water_hot_icon,
    color = (120, 191, 208),
    transportColor = (28, 84, 104),
    transportAccentColor = (245, 177, 134),
    canBeDiscarded = False,
    description = "Heavy Water (High) is a form of deuterium-water maintained at high pressures in the primary circuit of a CANDU reactor, where it serves as both a moderator and a coolant, exiting the reactor as a dense, hot, and energized fluid after absorbing heat from the core."
)

heat_exchanger_machine = build_machine(
    machineId            = "Heat_exchanger_candu",
    source               = "ThermalDesalinator",
    name = "Heat Exchanger",
    description = "A heat exchanger is a device that transfers thermal energy between two fluids while keeping them separate, allowing a circuit to be heated or cooled without mixing the substances involved.",
    research = "Research_Candu_Reactor",
    copy_recipes         = False
)

candu_rod_recipe = build_recipe(
    "Recipe_Candu_Rod",
    "CANDU Rod",
    "An industrial process that compresses yellowcake into a ceramic material and inserts it into a steel cladding, forming a ready-to-use CANDU fuel rod.",
    "ChemicalPlant2",
    research = candu_research,
    duration = Duration.FromSec(15),
    ingredients = [
        Product("Product_Yellowcake", Quantity(2)),
        Product("Product_Steel", Quantity(1))
    ],
    products = [
        Product("Product_Candu_rod", Quantity(2))
    ]
)

heavy_water_recipe = build_recipe(
    "Recipe_heavy_water",
    "Heavy Water",
    "An isotopic exchange process in which water, high-pressure steam, and acid react to concentrate deuterium. The separation yields heavy water as the enriched fraction and produces sour water as a byproduct to be treated or recycled.",
    "ChemicalPlant2",
    research = candu_research,
    duration = Duration.FromSec(30),
    ingredients = [
        Product("Product_Water", Quantity(120)),
        Product("Product_SteamHi", Quantity(40)),
        Product("Product_Acid", Quantity(15))
    ],
    products = [
        Product("Product_heavy_water", Quantity(100)),
        Product("Product_SourWater", Quantity(75))
    ]
)

primary_steam_generator_recipe = build_recipe(
    "Recipe_primary_steam_generator",
    "Primary Steam Generation",
    "Primary Steam Generation is the process in which heat from the pressurized heavy‑water primary loop is transferred to ordinary water, converting it into high‑pressure steam while the heavy water exits cooled but unchanged.",
    "Heat_exchanger_candu",
    research = candu_research,
    duration = Duration.FromSec(30),
    ingredients = [
        Product("Product_heavy_water_high", Quantity(192)),
        Product("Product_Water", Quantity(192))
    ],
    products = [
        Product("Product_heavy_water", Quantity(192)),
        Product("Product_SteamHi", Quantity(192))
    ]
)

heavy_water_unlock = add_unlock_product(
    "Research_Candu_Reactor",
    "Product_heavy_water"
)

heavy_water_hot_unlock = add_unlock_product(
    "Research_Candu_Reactor",
    "Product_heavy_water_high"
)

candu_rod_unlock = add_unlock_product(
    "Research_Candu_Reactor",
    "Product_Candu_rod"
)

heat_exchanger_unlock = add_unlock_machine(
    "Research_Candu_Reactor",
    heat_exchanger_machine
)

heavy_water_research = add_unlock_recipe(
    "Research_Candu_Reactor",
    "ChemicalPlant2",
    "Recipe_heavy_water"
)

candu_rod_unlock = add_unlock_recipe(
    "Research_Candu_Reactor",
    "ChemicalPlant2",
    "Recipe_Candu_Rod"
)

primary_steam_generator_research = add_unlock_recipe(
    "Research_Candu_Reactor",
    heat_exchanger_machine,
    "Recipe_primary_steam_generator"
)

candu_reactor_t1 = build_nuclear_reactor(
    reactorId              = "Candu_Reactor_T1_build",
    source                 = "NuclearReactor",
    name = "CANDU Nuclear Reactor",
    description = "A thermal CANDU reactor that sustains a nuclear chain reaction using enriched uranium rods or natural uranium rods. The reaction releases a large amount of energy, which is used to generate steam. At maximum power, this plant can generate up to 90 MW of electricity. Warning: Spent fuel is radioactive and can pose a health risk to the public if not stored in a specialized facility.",
    maxPowerLevel = 3,
    fuelCapacity = 40,
    minFuelToOperate = 16,
    processDurationSeconds = 10,
    computingConsumed = 0,
    fuel_pairs             = [
        FuelPair(fuelIn="Product_UraniumRod", spentFuelOut="Product_SpentFuel", durationSeconds=120),
        FuelPair(fuelIn="Product_Candu_rod", spentFuelOut="Product_SpentFuel", durationSeconds=120)
    ],
    coolantIn = "Product_heavy_water",
    coolantOut = "Product_heavy_water_high",
    waterInProduct = "Product_heavy_water",
    steamOutProduct = "Product_heavy_water_high",
    layout_str = "      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][5][5][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][5][6][7][7][7][7][7][7][7][7][6][5][5]   \n      [4][5][6][7][7][7][7][7][7][7][7][6][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][6][7][7][7][7][7][7][7][7][5][5][5]   \n   [3][4][4][6][7][7][7][7][7][7][7][7][5][5][5]   \nF#>[3][4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n   [3][4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \nS#<[3][4][4][4][4][4][6][6][6][6][4][4][4][4][4]   \n   [3][4][4][4][4][5][5][5][5][5][5][4][4][4][4][3]\n   [3][4][4][4][6][6][6][6][6][6][6][5][4][4][4][3]\n   [3][4][4][6][6]-3]-3]-3]-3][6][6][6][5][5][4][3]\n   [3][4][5][6]-3]-3]-5]-5]-3]-3]-3][6][6][5][4][3]\n   [3][4]-3]-3]-3]-5]-5]-5]-5]-5]-3]-3]-3][5][4][3]\n   [3][4]-4]-4]-5]-5]-5]-5]-5]-5]-4]-4]-3][5][4][3]\n   [3][4]-4]-4]-5]-5]-5]-5]-5]-5]-4]-4]-3][5][4][3]\n   [3][4]-3]-3]-5]-5]-5]-5]-5]-5]-3]-3]-3][5][4][3]\n   [3][4][5]-3]-3]-5]-5]-5]-5]-5]-3][6][6][5][4][3]\n   [3][4][4][5]-3]-3]-3]-3]-3]-3]-3][6][5][5][4][3]\n      [4][4][4][5][6][6][6][6][6][6][5][4][4][4]   \n         [4][4][4][5][5][5][5][5][5][4][4][4]      \n      D@>[4][4][4][4][4][4][4][4][4][4][4][4]W@>   \n         [4][4][4][4][4][4][4][4][4][4][4][4]      \n      A@>[4][4][4][4][4][4][4][4][4][4][4][4]X@>   \n      B@>[4][4][4][4][4][4][4][4][4][4][4][4]Y@>   \n         [4][4][4][4][4][4][4][4][4][4][4][4]      ",
    research = "Research_Candu_Reactor"
)

candu_reactor_t2 = build_nuclear_reactor(
    reactorId              = "Candu_Reactor_T2_Build",
    source                 = "NuclearReactorT2",
    name = "CANDU Nuclear Reactor II",
    description = "An advanced thermal CANDU reactor with higher energy efficiency, capable of using MOX fuel and automatically adjusting its power level (if rated power is available). At maximum power, this plant can generate up to 120 MW of electricity.",
    maxPowerLevel = 4,
    fuelCapacity = 40,
    minFuelToOperate = 16,
    processDurationSeconds = 10,
    computingConsumed = 12,
    fuel_pairs             = [
        FuelPair(fuelIn="Product_MoxRod", spentFuelOut="Product_SpentMox", durationSeconds=120),
        FuelPair(fuelIn="Product_UraniumRod", spentFuelOut="Product_SpentFuel", durationSeconds=120),
        FuelPair(fuelIn="Product_Candu_rod", spentFuelOut="Product_SpentFuel", durationSeconds=120)
    ],
    coolantIn = "Product_heavy_water",
    coolantOut = "Product_heavy_water_high",
    waterInProduct = "Product_heavy_water",
    steamOutProduct = "Product_heavy_water_high",
    layout_str = "      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][5][5][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][5][6][7][7][7][7][7][7][7][7][6][5][5]   \n      [4][5][6][7][7][7][7][7][7][7][7][6][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n      [4][4][6][7][7][7][7][7][7][7][7][5][5][5]   \n   [3][4][4][6][7][7][7][7][7][7][7][7][5][5][5]   \nF#>[3][4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \n   [3][4][4][4][7][7][7][7][7][7][7][7][5][5][5]   \nS#<[3][4][4][4][4][4][6][6][6][6][4][4][4][4][4]   \n   [3][4][4][4][4][5][5][5][5][5][5][4][4][4][4][3]\n   [3][4][4][4][6][6][6][6][6][6][6][5][4][4][4][3]\n   [3][4][4][6][6]-3]-3]-3]-3][6][6][6][5][5][4][3]\n   [3][4][5][6]-3]-3]-5]-5]-3]-3]-3][6][6][5][4][3]\n   [3][4]-3]-3]-3]-5]-5]-5]-5]-5]-3]-3]-3][5][4][3]\n   [3][4]-4]-4]-5]-5]-5]-5]-5]-5]-4]-4]-3][5][4][3]\n   [3][4]-4]-4]-5]-5]-5]-5]-5]-5]-4]-4]-3][5][4][3]\n   [3][4]-3]-3]-5]-5]-5]-5]-5]-5]-3]-3]-3][5][4][3]\n   [3][4][5]-3]-3]-5]-5]-5]-5]-5]-3][6][6][5][4][3]\n   [3][4][4][5]-3]-3]-3]-3]-3]-3]-3][6][5][5][4][3]\n      [4][4][4][5][6][6][6][6][6][6][5][4][4][4]   \n         [4][4][4][5][5][5][5][5][5][4][4][4]      \n      D@>[4][4][4][4][4][4][4][4][4][4][4][4]W@>   \n         [4][4][4][4][4][4][4][4][4][4][4][4]      \n      A@>[4][4][4][4][4][4][4][4][4][4][4][4]X@>   \n      B@>[4][4][4][4][4][4][4][4][4][4][4][4]Y@>   \n         [4][4][4][4][4][4][4][4][4][4][4][4]      ",
    research = "ResearchNuclearReactor2"
)
