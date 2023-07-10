using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Resourcerer.Api.Services;
using Resourcerer.Logic;
using Resourcerer.UnitTests.Utilities.TestClasses;

namespace Resourcerer.UnitTests.Api;

public class PipelineTests
{
    private readonly Pipeline _pipeline;
    public PipelineTests()
    {
        var fakeLogger = A.Fake<ILogger<Pipeline>>();
        _pipeline = new Pipeline(fakeLogger);
    }

    [Fact]
    public void Pipeline_Maps_OkResult()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto();

        var result = _pipeline.Pipe(handler, dto).GetAwaiter().GetResult();

        Assert.True(result is Ok<Unit>);
    }

    [Fact]
    public void Pipeline_Maps_ValidationErrors_To_BadRequest()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = eHandlerResult.Invalid };

        var result = _pipeline.Pipe(handler, dto).GetAwaiter().GetResult();

        var r = result as BadRequest<string[]>;
        Assert.NotNull(r);
        Assert.NotNull(r.Value);
        Assert.Contains(TestDto.ErrorMessage, r.Value);
    }

    [Fact]
    public void Pipeline_Uses_CustomMapper()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto();
        var customMapper = (Unit e) => Results.Accepted();

        var result = _pipeline.Pipe(handler, dto).GetAwaiter().GetResult();

        Assert.NotNull(result);
        Assert.True(result is Accepted);
    }

    [Fact]
    public void Pipeline_Skips_CustomMapper_When_Result_NotFound()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = eHandlerResult.NotFound };
        var customMapper = (Unit e) => Results.Accepted();

        var result = _pipeline.Pipe(handler, dto).GetAwaiter().GetResult();

        Assert.NotNull(result);
        Assert.True(result is NotFound<Unit>);
    }
}
