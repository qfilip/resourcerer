using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Resourcerer.Api.Services;
using Resourcerer.Logic;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.FluentMocks;
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
    public async Task Pipeline_Maps_OkResult()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto();

        var iResults = new[]
        {
            await _pipeline.Pipe(handler, dto),
            await _pipeline.Pipe<TestDto, TestDtoValidator, Unit>(handler, dto)
        };

        iResults.Every(x =>
        {
            Assert.True(x is Ok<Task<Unit>>);
        });
    }

    [Fact]
    public async Task Pipeline_Maps_ValidationErrors_To_BadRequest()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = eHandlerResult.Invalid };

        var iResults = new[]
        {
            await _pipeline.Pipe(handler, dto),
            await _pipeline.Pipe<TestDto, TestDtoValidator, Unit>(handler, dto)
        };

        iResults.Every(x =>
        {
            var r = x as BadRequest<string[]>;
            Assert.NotNull(r);
            Assert.NotNull(r.Value);
            Assert.Contains(TestDtoValidator.ErrorMessage, r.Value);
        });
    }

    [Fact]
    public async Task Pipeline_Uses_CustomMapper()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto();
        var customMapper = (Unit e) => Results.Accepted();

        var iResults = new[]
        {
             await _pipeline.Pipe(handler, dto, customMapper),
             await _pipeline
                .Pipe<TestDto, TestDtoValidator, Unit>(handler, dto, customMapper)
        };

        iResults.Every(x =>
        {
            Assert.NotNull(x);
            Assert.True(x is Accepted);
        });
    }

    [Fact]
    public async Task Pipeline_Skips_CustomMapper_When_Result_NotFound()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = eHandlerResult.NotFound };
        var customMapper = (Unit e) => Results.Accepted();

        var iResults = new[]
        {
             await _pipeline.Pipe(handler, dto, customMapper),
             await _pipeline
                .Pipe<TestDto, TestDtoValidator, Unit>(handler, dto, customMapper)
        };

        iResults.Every(x =>
        {
            Assert.NotNull(x);
            Assert.True(x is Accepted);
        });
    }
}
