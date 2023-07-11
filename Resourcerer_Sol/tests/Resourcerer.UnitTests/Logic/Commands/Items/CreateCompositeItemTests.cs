using Resourcerer.Logic.Commands.Items;

namespace Resourcerer.UnitTests.Logic.Commands.Items;

public class CreateCompositeItemTests : TestsBase
{
    private readonly CreateCompositeItem.Handler _handler;
    public CreateCompositeItemTests()
    {
        _handler = new(_testDbContext);
    }
}
