using FakeItEasy;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using Resourcerer.Api;
using Resourcerer.UnitTests.Utilities;
using Resourcerer.UnitTests.Utilities.FluentMocks;

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
        
        var result = await _pipeline.Pipe<TestDto, TestDtoValidator, TestEntity>(handler, dto);
        
        Assert.True(result is Ok<TestEntity>);
    }

    [Fact]
    public async Task Pipeline_Maps_ValidationErrors_To_BadRequest()
    {
        var handler = new TestHandler.Handler();
        var dto = new TestDto() { Property = 1 };

        var iResult = await _pipeline.Pipe<TestDto, TestDtoValidator, TestEntity>(handler, dto);
        var result = iResult as BadRequest<string[]>;
        
        Assert.NotNull(result);
        Assert.Contains(TestDtoValidator.ErrorMessage, result.Value!);
    }
}
