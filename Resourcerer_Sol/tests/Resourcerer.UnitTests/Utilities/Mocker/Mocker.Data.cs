using Resourcerer.DataAccess.Contexts;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    public static void MockDbData(AppDbContext context)
    {
        var material = MockCategory(context, x => x.Name = "material");
        var product = MockCategory(context, x => x.Name = "product");

        var cement = MockItem(context, x =>
        {
            x.Category = material;
            x.Name = "cement";
        }, 1);

        var sand = MockItem(context, x =>
        {
            x.Category = material;
            x.Name = "sand";
        }, 1);

        var mortar = MockItem(context, x =>
        {
            x.Category = product;
            x.Name = "mortar";
        }, 10);

        var hourglass = MockItem(context, x =>
        {
            x.Category = product;
            x.Name = "hourglass";
        }, 10);

        var meat = MockItem(context, x =>
        {
            x.Category = product;
            x.Name = "meat";
            x.ExpirationTimeSeconds = TimeSpan.FromDays(7).Seconds;
        });

        MockExcerpts(context, mortar, new[]
        {
            (cement, 2d),
            (sand, 2d)
        });

        MockExcerpts(context, hourglass, new[]
        {
            (sand, 2d)
        });

        context.SaveChanges();
        context.ChangeTracker.Clear();
    }
}
