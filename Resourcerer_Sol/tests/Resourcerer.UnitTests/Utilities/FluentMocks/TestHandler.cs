using Resourcerer.Logic;
using Resourcerer.UnitTests.Utilities.TestClasses;

namespace Resourcerer.UnitTests.Utilities.FluentMocks;

public static class TestHandler
{
    public class Handler : IRequestHandler<TestDto, TestEntity>
    {
        public Task<HandlerResult<TestEntity>> Handle(TestDto request)
        {
            return Task.FromResult(HandlerResult<TestEntity>.Ok(new TestEntity()));
        }
    }
}
