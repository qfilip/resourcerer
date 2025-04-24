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

        //var ctgMaterials = forger.Fake<Category>(x =>
        //{
        //    x.Name = "Materials";
        //    x.Company = company;
        //});

        //var ctgWeapons = forger.Fake<Category>(x =>
        //{
        //    x.Name = "Weapons";
        //    x.Company = company;
        //});

        //var ctgMelee = forger.Fake<Category>(x =>
        //{
        //    x.Name = "Melee";
        //    x.Company = company;
        //    x.ParentCategory = ctgWeapons;
        //});

        //var uomItem = forger.Fake<UnitOfMeasure>(x =>
        //{
        //    x.Name = "Piece";
        //    x.Symbol = "p";
        //});

        //var cSword = forger.Fake<Item>(x =>
        //{
        //    x.Name = "Sword";
        //    x.UnitOfMeasure = uomItem;
        //    x.Category = ctgMelee;
        //});

        //var cWood = forger.Fake<Item>(x =>
        //{
        //    x.Name = "Wood";
        //    x.UnitOfMeasure = uomItem;
        //    x.Category = ctgMaterials;
        //});

        //var cIron = forger.Fake<Item>(x =>
        //{
        //    x.Name = "Iron";
        //    x.UnitOfMeasure = uomItem;
        //    x.Category = ctgMaterials;
        //});

        //(Item element, double qty)[] elements = [(cWood, 0.2d), (cIron, 1d)];

        //FakeRecipe(forger, cSword, elements);

        //forger.Fake<Price>(x => { x.Item = cSword; x.UnitValue = 2d; });
        //forger.Fake<Price>(x => { x.Item = cWood; x.UnitValue = 0.2d; });
        //forger.Fake<Price>(x => { x.Item = cIron; x.UnitValue = 1d; });
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
