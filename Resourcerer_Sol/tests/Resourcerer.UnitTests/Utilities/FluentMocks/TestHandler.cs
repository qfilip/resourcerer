using Resourcerer.Logic;
using Resourcerer.UnitTests.Utilities.TestClasses;

namespace Resourcerer.UnitTests.Utilities.FluentMocks;

public static class TestHandler
{
    public class Handler : IRequestHandler<TestDto, Unit>
    {
        public Task<HandlerResult<Unit>> Handle(TestDto request)
        {
            if(request.Property == eHandlerResult.Ok)
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
