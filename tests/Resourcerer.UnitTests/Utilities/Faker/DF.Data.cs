using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.UnitTests.Utilities.Faker;

internal static partial class DF
{
    public static void MockDbData(AppDbContext context)
    {
        var material = FakeCategory(context, x => x.Name = "material");
        var product = FakeCategory(context, x => x.Name = "product");

        var cement = FakeItem(context, x =>
        {
            x.Category = material;
            x.Name = "cement";
        }, 1);

        var sand = FakeItem(context, x =>
        {
            x.Category = material;
            x.Name = "sand";
        }, 1);

        var mortar = FakeItem(context, x =>
        {
            x.Category = product;
            x.Name = "mortar";
        }, 10);

        var hourglass = FakeItem(context, x =>
        {
            x.Category = product;
            x.Name = "hourglass";
        }, 10);

        var meat = FakeItem(context, x =>
        {
            x.Category = product;
            x.Name = "meat";
            x.ExpirationTimeSeconds = TimeSpan.FromDays(7).Seconds;
        });

        FakeExcerpts(context, mortar, new[]
        {
            (cement, 2d),
            (sand, 2d)
        });

        FakeExcerpts(context, hourglass, new[]
        {
            (sand, 2d)
        });

        context.SaveChanges();
        context.ChangeTracker.Clear();
    }
}
