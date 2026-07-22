from Mafi import Duration, Quantity
from Mafi.Base import Assets, Ids
from CustomAssets import add_loose_product_material, add_texture, build_product_loose, build_recipe, Product, Port

# Pile material for our new loose product. Only the albedo is overridden; normals and the
# smooth/metal channel are inherited from the default reference material so the surface
# still reads as rocky/granular like the vanilla pile.
filter_media_mat = add_loose_product_material(
    path   = "Assets/AirFiltering/AirFilterIL_mat",
    albedo = "Assets/Products/FilterMediaIronLime.png",
    tiling = 8
)

# Icon for the new product. Reuse the vanilla filter media icon since it's close enough.
filter_media_icon = add_texture("Assets/Products/FilterMediaIronLimeIcon.png")

# New loose product: iron-limestone filter media. Composition differs from vanilla
# Product_FilterMedia (iron ore + limestone instead of vanilla inputs), so it has its own id.
#
# particleColor is set explicitly because LooseProductMaterialManager auto-derives it from the
# average of the pile albedo texture during ITS ctor — but our material isn't in AssetsDb at
# that point (CustomAssetManager runs after LPMM and patches the texture array slice
# retroactively), so the auto-derive samples the fallback material instead. Pin it to a value
# that matches the iron-limestone albedo to get the right conveyor-spill particle color.
build_product_loose(
    productId = "Product_FilterMediaIronLime",
    name      = "Filter media (iron + limestone)",
    icon      = filter_media_icon,
    material  = filter_media_mat,
    description = "Filter media produced from iron ore and limestone. Cheaper alternative to vanilla filter media when iron ore is plentiful.",
    particleColor = (170, 165, 155),
    isStorable = True,
    isRough = True,
    dumpsAs = Ids.TerrainMaterials.Gravel
)

# Producer recipe: 3 iron ore + 1 limestone -> 4 iron-limestone filter media.
# Lossless mixing in the Industrial Mixer; auto-available with the machine.
build_recipe(
    "CustomRecipe_AirFilterIL_Mixing",
    "Filter media (iron + limestone) mixing",
    "Mix 1 iron ore + 3 limestone into 4 filter media. More efficient than vanilla filter media production when iron ore is plentiful.",
    Ids.Machines.IndustrialMixer,
    duration = Duration.FromSec(30),
    ingredients = [
        Product(Ids.Products.IronOre, Quantity(1)),
        Product(Ids.Products.Limestone, Quantity(3))
    ],
    products = [
        Product("Product_FilterMediaIronLime", Quantity(4))
    ]
)

# Producer recipe: 3 iron ore + 1 limestone -> 4 iron-limestone filter media.
# Lossless mixing in the Industrial Mixer; auto-available with the machine.
build_recipe(
    "CustomRecipe_AirFilterIL_Mixing_T2",
    "Filter media (iron + limestone) mixing",
    "Mix 2 iron ore + 6 limestone into 4 filter media. More efficient than vanilla filter media production when iron ore is plentiful.",
    Ids.Machines.IndustrialMixerT2,
    duration = Duration.FromSec(30),
    ingredients = [
        Product(Ids.Products.IronOre, Quantity(2)),
        Product(Ids.Products.Limestone, Quantity(6))
    ],
    products = [
        Product("Product_FilterMediaIronLime", Quantity(8))
    ]
)

# Consumer recipe: add next recipe to air crubber research
build_recipe(
    "CustomRecipe_AirFilterIL_Scubbing",
    "Air scrubber filter media (iron + limestone) consumption",
    "Use 4 iron-limestone filter media to scrub air. More efficient than vanilla filter media consumption when iron ore is plentiful.",
    Ids.Machines.ExhaustScrubber,
    research = Ids.Research.ExhaustFiltration,
    duration = Duration.FromSec(20),
    ingredients = [
        Product(Ids.Products.Exhaust, Quantity(160)),
        Product(Ids.Products.Water, Quantity(16)),
        Product("Product_FilterMediaIronLime", Quantity(2))
    ],
    products = [
        Product(Ids.Products.Sulfur, Quantity(4), "Z"),
        Product(Ids.Products.CarbonDioxide, Quantity(64), "X"),
        Product(Ids.Products.SteamLo, Quantity(16)),
        Product(Ids.Products.Slag, Quantity(2))
    ]
)

# Allow to burn wood directly
unit_burner = build_machine(
    machineId            = "BoilerWood",
    source               = "BoilerCoal",
    name = "Boiler (unit fuels)",
    ports                = [
        Port(name="B", type="input", shape="IoPortShape_Pipe", position=(0, 2, 0), direction="-X"),
        Port(name="A", type="input", shape="IoPortShape_FlatConveyor", position=(0, 1, 0), direction="-X"),
        Port(name="X", type="output", shape="IoPortShape_Pipe", position=(4, 1, 0), direction="+X"),
        Port(name="Y", type="output", shape="IoPortShape_Pipe", position=(2, 0, 0), direction="-Y")
    ],
    copy_recipes         = False,
    copy_ports           = False
)

build_recipe(
    "BoilerWood_WoodBurning",
    "Burning Wood",
    "",
    unit_burner,
    duration = Duration.FromSec(20),
    ingredients = [
        Product("Product_Water", Quantity(8)),
        Product("Product_Wood", Quantity(9))
    ],
    products = [
        Product("Product_SteamHi", Quantity(8)),
        Product("Product_Exhaust", Quantity(7))
    ]
)

