using Resourcerer.Logic;

namespace Resourcerer.UnitTests.Utilities.TestClasses;

public static class TestHandler
{
    public class Handler : IAppHandler<TestDto, Unit>
    {
        public Task<HandlerResult<Unit>> Handle(TestDto request)
        {
            if (request.Property == eHandlerResult.Ok)
            {
                return Task.FromResult(HandlerResult<Unit>.Ok(new Unit()));
            }
            else if (request.Property == eHandlerResult.NotFound)
            {
                return Task.FromResult(HandlerResult<Unit>.NotFound("Oops"));
            }
            else
            {
                return Task.FromResult(HandlerResult<Unit>.ValidationError(TestDtoValidator.ErrorMessage));
            }
        }
    }
}
