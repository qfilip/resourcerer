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
            x.CategoryId = material.Id;
            x.Name = "cement";
        });

        var sand = MockItem(context, x =>
        {
            x.CategoryId = material.Id;
            x.Name = "sand";
        });

        var mortar = MockItem(context, x =>
        {
            x.CategoryId = product.Id;
            x.Name = "mortar";
        });

        var hourglass = MockItem(context, x =>
        {
            x.CategoryId = product.Id;
            x.Name = "hourglass";
        });

        var excerptList1 = MockExcerpts(context, mortar, new[]
        {
            (cement, 2d),
            (sand, 2d)
        });

        var excerptList2 = MockExcerpts(context, hourglass, new[]
        {
            (sand, 2d)
        });
    }
}
