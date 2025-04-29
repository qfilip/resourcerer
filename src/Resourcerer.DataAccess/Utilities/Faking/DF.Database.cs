using Resourcerer.DataAccess.Entities;
using SqlForgery;

namespace Resourcerer.DataAccess.Utilities.Faking;

public static partial class DF
{
    public static void FakeDatabase(Forger forger, string? permissions = "{}")
    {
        var company = forger.Fake<Company>(x =>
        {
            x.Name = "Tropical Trades";
        });

        var appUser = forger.Fake<AppUser>(x =>
        {
            x.Name = "ss";
            x.IsAdmin = true;
            x.Email = "a@a.com";
            x.DisplayName = "Vaas Montenegro";
            x.PasswordHash = Resourcerer.Utilities.Cryptography.Hasher.GetSha256Hash("1");
            x.Company = company;
            x.Permissions = permissions;
        });

        var ctgFood = forger.Fake<Category>(x =>
        {
            x.Name = "Food";
            x.Company = company;
        });

        var ctgDrinks = forger.Fake<Category>(x =>
        {
            x.Name = "Drink";
            x.Company = company;
        });

        var ctgVegetables = forger.Fake<Category>(x =>
        {
            x.Name = "Vegetable";
            x.Company = company;
            x.ParentCategory = ctgFood;
        });

        var uomKg = forger.Fake<UnitOfMeasure>(x =>
        {
            x.Name = "Kilogram";
            x.Symbol = "Kg";
            x.Company = company;
        });

        var itemTomato = forger.Fake<Item>(x =>
        {
            x.Name = "Tomato";
            x.UnitOfMeasure = uomKg;
            x.Category = ctgVegetables;
        });

        var itemCabbage = forger.Fake<Item>(x =>
        {
            x.Name = "Cabbage";
            x.UnitOfMeasure = uomKg;
            x.Category = ctgVegetables;
        });

        var itemSalad = forger.Fake<Item>(x =>
        {
            x.Name = "Salad";
            x.UnitOfMeasure = uomKg;
            x.Category = ctgFood;
        });

        (Item element, double qty)[] saladElements = [(itemTomato, 0.2d), (itemCabbage, 1d)];

        FakeRecipe(forger, itemSalad, saladElements);

        forger.Fake<Price>(x => { x.Item = itemTomato; x.UnitValue = 2d; });
        forger.Fake<Price>(x => { x.Item = itemCabbage; x.UnitValue = 0.1d; });
        forger.Fake<Price>(x => { x.Item = itemSalad; x.UnitValue = 10d; });
    }

    private static void FakeRecipe(Forger forger, Item composite, (Item element, double qty)[] elements)
    {
        forger.Fake<Recipe>(x =>
        {
            x.RecipeExcerpts = elements.Select(e =>
                forger.Fake<RecipeExcerpt>(re =>
                {
                    re.Quantity = e.qty;
                    re.Element = e.element;
                    re.Recipe = x;
                })
            ).ToList();
        });
    }
}
