using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities.Mocker;

internal static partial class Mocker
{
    internal static ElementPurchasedEvent MockElementPurchasedEvent(AppDbContext context)
    {
        var element = Mocker.MockElement(context);
        

        return new()
        {
            Id = Guid.NewGuid(),
            ElementId = element.Id
        };
    }
}
