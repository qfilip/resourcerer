﻿using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    internal static void FakeDatabase(TestDbContext ctx, Action<TestDbContext>? exporter = null)
    {
        var company = Fake<Company>(ctx, x =>
        {
            x.Name = "Item shop";
        });

        var appUser = Fake<AppUser>(ctx, x =>
        {
            x.Name = "Shopkeeper";
            x.PasswordHash = Resourcerer.Utilities.Cryptography.Hasher.GetSha256Hash("123");
            x.Company = company;
        });

        var ctgMaterials = Fake<Category>(ctx, x =>
        {
            x.Name = "Materials";
            x.Company = company;
        });

        var ctgWeapons = Fake<Category>(ctx, x =>
        {
            x.Name = "Weapons";
            x.Company = company;
        });

        var ctgMelee = Fake<Category>(ctx, x =>
        {
            x.Name = "Melee";
            x.Company = company;
            x.ParentCategory = ctgWeapons;
        });

        var uomItem = Fake<UnitOfMeasure>(ctx, x =>
        {
            x.Name = "Piece";
            x.Symbol = "p";
        });

        var cSword = Fake<Item>(ctx, x =>
        {
            x.Name = "Sword";
            x.UnitOfMeasure = uomItem;
            x.Category = ctgMelee;
        });

        var cWood = Fake<Item>(ctx, x =>
        {
            x.Name = "Wood";
            x.UnitOfMeasure = uomItem;
            x.Category = ctgMaterials;
        });

        var cIron = Fake<Item>(ctx, x =>
        {
            x.Name = "Iron";
            x.UnitOfMeasure = uomItem;
            x.Category = ctgMaterials;
        });

        (Item element, double qty)[] elements = [(cWood, 0.2d), (cIron, 1d)];

        FakeExcerpts(ctx, cSword, elements);

        Fake<Price>(ctx, x => { x.Item = cSword; x.UnitValue = 2d; });
        Fake<Price>(ctx, x => { x.Item = cWood; x.UnitValue = 0.2d; });
        Fake<Price>(ctx, x => { x.Item = cIron; x.UnitValue = 1d; });

        ctx.SaveChanges();

        exporter?.Invoke(ctx);
    }

    private static void FakeExcerpts(TestDbContext ctx, Item composite, (Item element, double qty)[] elements)
    {
        foreach (var item in elements)
        {
            Fake<Excerpt>(ctx, x =>
            {
                x.Quantity = item.qty;
                x.Composite = composite;
                x.Element = item.element;
            });
        }
    }
}
