using Resourcerer.Logic.Commands.Composites;

namespace Resourcerer.UnitTests.Logic.Commands.Composites;

public class CreateCompositeTests : TestsBase
{
    private readonly CreateComposite.Handler _handler;
    public CreateCompositeTests()
    {
        _handler = new CreateComposite.Handler(_testDbContext);
    }

    [Fact]
    public void When_RequiredElements_Exists_Then_Ok()
    {

    }

    [Fact]
    public void When_RequiredElements_NotFound_Then_ValidationError()
    {

    }

    [Fact]
    public void When_ElementWithNoPriceUsed_Then_ValidationError()
    {

    }

    [Fact]
    public void When_Composite_WithSameName_Exists_Then_ValidationError()
    {

    }
}
