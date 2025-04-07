using Resourcerer.DataAccess.Entities;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.V1;

namespace Resourcerer.UnitTests.Logic.V1.Examples;

public class GetExamplesTests : TestsBase
{
    private readonly GetExamples.Handler _sut;
    public GetExamplesTests()
    {
        _sut = new(_ctx, GetMapster());
    }

    [Fact]
    public async Task HappyPath__Ok()
    {
        // arrange
        var count = 3;
        for (int i = 0; i < count; i++)
            _forger.Fake<ExampleEntity>(x => x.Text = $"example-{i}");
        
        _ctx.SaveChanges();

        // act
        var result = await _sut.Handle(Unit.New);

        // assert
        Assert.Multiple(
            () => Assert.Equal(eHandlerResultStatus.Ok, result.Status),
            () => Assert.NotNull(result.Object),
            () => Assert.Equal(count, result.Object!.Length)
        );
    }
}
