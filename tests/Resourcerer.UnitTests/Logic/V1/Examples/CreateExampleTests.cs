using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;
using SqlForgery;

namespace Resourcerer.UnitTests.Logic.V1.Examples;

public class CreateExampleTests : TestsBase
{
    private readonly CreateExample.Handler _sut;
    public CreateExampleTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public async Task HappyPath__Ok()
    {
        // arrange
        var command = new V1CreateExampleCommand { Text = "test" };

        var result = await _sut.Handle(command);

        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var entities = _ctx.Examples.ToArray();
                Assert.Single(entities);
            }
        );
    }
}
