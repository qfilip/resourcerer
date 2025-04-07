using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;

namespace Resourcerer.UnitTests.Logic.V1.Examples;

public class UpdateExampleTests : TestsBase
{
    private readonly UpdateExample.Handler _sut;
    public UpdateExampleTests()
    {
        _sut = new(_ctx);
    }

    [Fact]
    public async Task HappyPath__Ok()
    {
        // arrange
        var existing = _forger.Fake<ExampleEntity>();
        var command = new V1UpdateExampleCommand
        { 
            ExampleId = existing.Id,
            NewText = "new"
        };

        var result = await _sut.Handle(command);

        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () =>
            {
                var entities = _ctx.Examples.ToArray();
                Assert.Single(entities);
                Assert.Equal(command.NewText, entities[0].Text);
            }
        );
    }

    [Fact]
    public async Task NotFound__NotFound()
    {
        // arrange
        var existing = _forger.Fake<ExampleEntity>();
        var command = new V1UpdateExampleCommand
        {
            ExampleId = Guid.NewGuid(),
            NewText = "new"
        };

        var result = await _sut.Handle(command);

        Assert.Equal(eHandlerResultStatus.NotFound, result.Status);
    }
}
