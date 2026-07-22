from Mafi import Duration, Quantity
from Mafi.Base import Assets, Ids
from CustomAssets import add_loose_product_material, add_texture, build_product_loose, build_recipe, Product

bioethanol_recipe = build_recipe(
    "Recipe_Bioethanol",
    "Bioethanol",
    "Bioethanol is an ethyl alcohol derived from plants, produced through the fermentation of biomass such as corn, sugarcane, grains, or agricultural residues. It is considered a renewable biofuel and is used both as a gasoline additive and in household stoves and fireplaces, thanks to its clean combustion, which produces mainly water vapor and carbon dioxide.",
    "FermentationTank",
    research = "ResearchSugarCane",
    duration = Duration.FromSec(30),
    ingredients = [
        Product("Product_Sugar", Quantity(8)),
        Product("Product_CarbonDioxide", Quantity(8)),
        Product("Product_Water", Quantity(8))
    ],
    products = [
        Product("Product_Ethanol", Quantity(12)),
        Product("Product_WasteWater", Quantity(12))
    ]
)

organic_pesticide_recipe = build_recipe(
    "Recipe_Organic_pesticide",
    "Organic pesticide",
    "Organic pesticides are pest control products derived from natural ingredients of botanical, microbial, or mineral origin. Although they are chemical compounds, they are derived from natural sources and tend to break down more quickly in the environment than synthetic pesticides, making them generally less persistent and less harmful to ecosystems and human health.",
    "BioProcessor",
    research = "ResearchSugarCane",
    duration = Duration.FromSec(30),
    ingredients = [
        Product("Product_Ethanol", Quantity(4)),
        Product("Product_Biomass", Quantity(4))
    ],
    products = [
        Product("Product_Pesticide", Quantity(8))
    ]
)

