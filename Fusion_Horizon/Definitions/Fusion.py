from Mafi import Duration, Quantity
from Mafi.Base import Assets, Ids
from CustomAssets import add_loose_product_material, add_texture, build_product_loose, build_recipe, Product

deuterium_icon = add_texture("Assets/Products/Deuterium.png")

helium_icon = add_texture("Assets/Products/Helium.png")

heliumhigh_icon = add_texture("Assets/Products/HeliumHigh.png")

tririum_icon = add_texture("Assets/Products/Tritium.png")

dtfuel_icon = add_texture("Assets/Products/DTFuel.png")

plasmadirty_icon = add_texture("Assets/Products/PlasmaDirty.png")

plasma_icon = add_texture("Assets/Products/Plasma.png")

plasmarefined_icon = add_texture("Assets/Products/PlasmaRefined.png")

fusion_research = build_research(
    "Research_Fusion_Reactor",
    "Fusion Nuclear Reactor",
    "Research into nuclear fusion reactors unlocks the production chain for tritium and deuterium, which are used as fuel in nuclear fusion reactors. It also unlocks the production of helium, which is used as a coolant in nuclear fusion reactors.",
    costs = 9000,
    position = (164, 50),
    icon = dtfuel_icon,
    tier = "Product_LabEquipment4"
)

product_deuterium = build_product_fluid(
    productId = "Product_deuterium",
    name      = "Deuterium",
    icon      = deuterium_icon,
    color = (67, 211, 219),
    transportColor = (40, 111, 216),
    transportAccentColor = (195, 207, 247),
    canBeDiscarded = False,
    description = "Deuterium is the heavy isotope of hydrogen, which exists in gaseous form as D₂ molecules. It is a light, stable, and non-radioactive gas used in nuclear and scientific applications due to its unique isotopic properties.",
    isStorable = True
)

product_helium = build_product_fluid(
    productId = "Product_helium",
    name      = "Helium",
    icon      = helium_icon,
    color = (138, 156, 160),
    transportColor = (71, 149, 154),
    transportAccentColor = (5, 66, 54),
    description = "Helium is an extremely light, inert, and non-reactive noble gas. It is stable, non-flammable, and is used in cryogenic, scientific, and industrial applications due to its low density and complete lack of chemical reactivity.",
    isStorable = True
)

product_helium__hot = build_product_fluid(
    productId = "Product_helium_hot",
    name      = "Helium (High)",
    icon      = heliumhigh_icon,
    color = (138, 156, 160),
    transportColor = (71, 149, 154),
    transportAccentColor = (245, 177, 134),
    canBeDiscarded = False,
    description = "Helium (High) at High Pressure is a compressed noble gas brought to high temperatures while retaining its complete chemical inertness. Under these conditions, it becomes an extremely stable thermal fluid, ideal for advanced cooling systems, heat transfer, and industrial processes that require a light, clean gas resistant to extreme temperatures."
)

product_tritium = build_product_fluid(
    productId = "Product_tritium",
    name      = "Tritium",
    icon      = tririum_icon,
    color = (130, 77, 194),
    transportColor = (176, 39, 166),
    transportAccentColor = (196, 157, 212),
    canBeDiscarded = False,
    description = "Tritium is a radioactive isotope of hydrogen, consisting of one proton and two neutrons. It is a light, unstable gas that emits weak beta radiation and is used in nuclear and scientific applications, as well as in the production of self-sustaining light sources."
)

product_dtfuel = build_product_fluid(
    productId = "Product_dtfuel",
    name      = "Fusion Core Fuel",
    icon      = dtfuel_icon,
    color = (141, 63, 227),
    transportColor = (58, 95, 138),
    transportAccentColor = (252, 196, 255),
    canBeDiscarded = False,
    description = "Fusion Core Fuel is an advanced fuel for fusion reactors, consisting of an optimized mixture of deuterium and tritium. It provides high energy density and enables highly efficient fusion reactions, serving as the foundation for next-generation energy systems."
)

product_plasmadirty = build_product_fluid(
    productId = "Product_plasmadirty",
    name      = "Plasma Dirty",
    icon      = plasmadirty_icon,
    color = (115, 0, 138),
    transportColor = (61, 90, 132),
    transportAccentColor = (43, 50, 61),
    canBeDiscarded = False,
    description = "An ionized fluid extracted from a fusion reactor, containing gaseous impurities such as deuterium, helium, and tritium. The mixture exhibits unstable behavior and irregular luminosity due to the presence of light nuclei that have not fully fused. It is used as a byproduct of the process or for studies of isotopic recombination."
)

product_plasmarefined = build_product_fluid(
    productId = "Product_plasmarefined",
    name      = "Refined Plasma",
    icon      = plasmarefined_icon,
    color = (185, 0, 222),
    transportColor = (95, 143, 212),
    transportAccentColor = (0, 62, 157),
    canBeDiscarded = False,
    description = "High-pressure plasma obtained by purifying raw plasma. It still contains residual impurities and helium, making it more stable but not sufficiently pure. It requires a further refining step before it can be used in non-nuclear industrial processes."
)

product_plasma = build_product_fluid(
    productId = "Product_plasma",
    name      = "Plasma",
    icon      = plasma_icon,
    color = (226, 88, 255),
    transportColor = (163, 0, 163),
    transportAccentColor = (255, 133, 169),
    description = "Fully purified plasma obtained from the final stage of refining treated plasma. It is free of impurities and has a stable composition, making it suitable for use in advanced industrial processes.",
    isStorable = True
)

deuterium_recipe = build_recipe(
    "Recipe_deuterium",
    "Deuterium",
    "",
    "ElectrolyzerT2",
    research = fusion_research,
    duration = Duration.FromSec(15),
    ingredients = [
        Product("Product_heavy_water", Quantity(20))
    ],
    products = [
        Product("Product_deuterium", Quantity(15)),
        Product("Product_Oxygen", Quantity(5))
    ]
)

tririum_recipe = build_recipe(
    "Recipe_tritium",
    "Tritium",
    "",
    "UraniumEnrichmentPlant",
    research = fusion_research,
    duration = Duration.FromSec(15),
    ingredients = [
        Product("Product_RefinedLithium", Quantity(20)),
        Product("Product_BlanketFuelEnriched", Quantity(15))
    ],
    products = [
        Product("Product_tritium", Quantity(19)),
        Product("Product_helium", Quantity(15)),
        Product("Product_FissionProduct", Quantity(1))
    ]
)

dtfuel_recipe = build_recipe(
    "Recipe_dtfuel",
    "Fusion Core Fuel",
    "Fusion Core Fuel is produced through the controlled combination of deuterium and tritium, which are purified and compressed to create a high-energy isotopic mixture. This compound ensures stability during storage and maximum efficiency in initiating fusion, making it the operational standard for advanced next-generation reactors.",
    "ChemicalPlant2",
    research = fusion_research,
    duration = Duration.FromSec(15),
    ingredients = [
        Product("Product_deuterium", Quantity(15)),
        Product("Product_tritium", Quantity(15))
    ],
    products = [
        Product("Product_dtfuel", Quantity(30))
    ]
)

primary_steam_generator_helium_recipe = build_recipe(
    "Recipe_primary_steam_generator_helium",
    "Primary Steam Generation (Helium)",
    "Primary Steam Generation is the process in which heat from the pressurized helium primary loop is transferred to ordinary water, converting it into high‑pressure steam while the heavy water exits cooled but unchanged.",
    "Heat_exchanger_candu",
    research = fusion_research,
    duration = Duration.FromSec(30),
    ingredients = [
        Product("Product_helium_hot", Quantity(192)),
        Product("Product_Water", Quantity(192))
    ],
    products = [
        Product("Product_helium", Quantity(192)),
        Product("Product_SteamSp", Quantity(192))
    ]
)

refinedplasma_recipe = build_recipe(
    "Recipe_refinedplasma",
    "Refined Plasma",
    "A high-pressure filtration process that treats contaminated plasma by selectively separating deuterium and tritium, producing a recoverable and reusable mixture. The residual plasma is partially purified and ready for further refinement.",
    "UraniumEnrichmentPlant",
    duration = Duration.FromSec(15),
    ingredients = [
        Product("Product_plasmadirty", Quantity(15))
    ],
    products = [
        Product("Product_plasmarefined", Quantity(12)),
        Product("Product_dtfuel", Quantity(3))
    ]
)

plasma_recipe = build_recipe(
    "Recipe_plasma",
    "Pure Plasma",
    "An advanced purification process that treats refined plasma by selectively removing residual helium and the remaining impurities. The resulting plasma achieves a stable and controlled composition, making it fully suitable for use in non-nuclear industrial processes.",
    "UraniumEnrichmentPlant",
    duration = Duration.FromSec(15),
    ingredients = [
        Product("Product_plasmarefined", Quantity(12))
    ],
    products = [
        Product("Product_plasma", Quantity(10)),
        Product("Product_helium", Quantity(2))
    ]
)

fusion_reactor_machine = build_nuclear_reactor(
    reactorId              = "Fusion_Reactor_t1",
    source                 = "FastBreederReactor",
    name = "Fusion Nuclear Reactor ",
    description = "The nuclear fusion reactor is an advanced system designed to fuse light nuclei, such as deuterium and tritium, generating enormous amounts of clean energy. It works by creating extreme conditions of temperature and pressure that allow the nuclei to fuse, releasing energy in a continuous and controlled manner. It represents the most promising technology for achieving a stable, safe, and virtually inexhaustible energy source. It operates at higher temperatures to produce very high-pressure steam (800 °C). If the core overheats and emergency cooling is unavailable, the reactor automatically shuts down by draining the molten fuel, resulting in the total loss of the fuel and damage to the reactor. At maximum power, this plant can supply up to 240 MW of electrical energy.",
    maxPowerLevel = 4,
    fuelCapacity = 160,
    minFuelToOperate = 80,
    processDurationSeconds = 10,
    computingConsumed = 18,
    fuel_pairs             = [
        FuelPair(fuelIn="Product_dtfuel", spentFuelOut="Product_helium", durationSeconds=15)
    ],
    coolantIn = "Product_helium",
    coolantOut = "Product_helium_hot",
    waterInProduct = "Product_helium",
    steamOutProduct = "Product_helium_hot",
    enrichment             = Enrichment(inputProduct="Product_dtfuel", outputProduct="Product_plasmadirty"),
    layout_str = "   [3][3][3][3][3][3][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]\n   [3][3][3][3][3][3][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][5][5][5][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\nF@>[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\nS@<[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\nQ@>[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\nE@<[6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][6][6][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [6][6][6][6][6][6][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [9![9![9![4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [9![9![9![4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [2][3][3][4][4][4][8][8][8][8][8][8][8][8][8][8][8][6][4][4][2]\n   [2][3][3][4][4][4][7][7][7][7][7][7][7][7][7][7][7][6][4][4][2]\n   [2][3][3][4][4][4][7][7][7][7][7][7][7][7][7][7][7][7][4][4][2]\n   [2][3][3][4][4][4][7][7][7][7][9][9][9][9][9][9][7][7][7][4][2]\n   [2][3][3][4][4][4][7][7][7][9][9][9][9][9][9][9][9][7][7][7][2]\n   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]\n   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]\n   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]\n   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7][2]\n   [2][3][3][4][4][4][7][7][9][7![7![7![7![7![7![7![7![9][7][7]   \n                  [4][7][7][9][9][9][7![7![7![7![9][9][9][7][7]   \n                     [7][7][7][9][9][9][9][9][9][9][9][7][7][7]   \n                        [7][7][7][9][9][9][9][9][9][7][7][7]      \n                        [7][7][7][7][7][7][7][7][7][7][7][7]      \n                     D@>[4][4][4][7][7][7][7][7][7][4][4][4]W@>   \n                        [4][4][4][4][4][4][4][4][4][4][4][4]      \n                     A@>[4][4][4][4][4][4][4][4][4][4][4][4]X@>   \n                     B@>[4][4][4][4][4][4][4][4][4][4][4][4]Y@>   \n                        [4][4][4][4][4][4][4][4][4][4][4][4]      ",
    research = "Research_Fusion_Reactor"
)

unlock_deuterium = add_unlock_product(
    "Research_Fusion_Reactor",
    "Product_deuterium"
)

unlock_tritium = add_unlock_product(
    "Research_Fusion_Reactor",
    "Product_tritium"
)

unlock_helium = add_unlock_product(
    "Research_Fusion_Reactor",
    "Product_helium"
)

unlock_helium_hot = add_unlock_product(
    "Research_Fusion_Reactor",
    "Product_helium_hot"
)

unlock_dtfuel = add_unlock_product(
    "Research_Fusion_Reactor",
    "Product_dtfuel"
)
