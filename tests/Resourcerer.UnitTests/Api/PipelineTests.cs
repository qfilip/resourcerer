using FakeItEasy;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Resourcerer.Api.Services;
using Resourcerer.Logic;
using Resourcerer.UnitTests.Utilities;
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
    public void When_Ok_Returns_Ok()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto();

        var result = _pipeline.Pipe(handler, dto).Await();

        Assert.True(result is Ok<Unit>);
    }

    [Fact]
    public void When_DtoValidationFails_Returns_BadRequest()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = eHandlerResult.Invalid };

        var result = _pipeline.Pipe(handler, dto).Await();

        var r = result as BadRequest<IEnumerable<string>>;
        Assert.NotNull(r);
        Assert.NotNull(r.Value);
        Assert.Contains(TestDto.ErrorMessage, r.Value);
    }

    [Fact]
    public void When_CustomMapperIsUsed_Returns_MappedHttpResult()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto();
        var customMapper = (Unit e) => Results.Accepted();

        var result = _pipeline.Pipe(handler, dto, customMapper).Await();

        Assert.NotNull(result);
        Assert.True(result is Accepted);
    }

    [Fact]
    public void When_CustomMapperIsUsed_And_HandlerResult_Is_NotFound_Returns_NotFound()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = eHandlerResult.NotFound };
        var customMapper = (Unit e) => Results.Accepted();

        var result = _pipeline.Pipe(handler, dto, customMapper).Await();

        Assert.NotNull(result);
        Assert.True(result is NotFound<Unit>);
    }
}
