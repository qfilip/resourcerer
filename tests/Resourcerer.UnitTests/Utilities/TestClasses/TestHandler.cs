using FluentValidation.Results;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;

namespace Resourcerer.UnitTests.Utilities.TestClasses;

public static class TestHandler
{
    public class Handler : IAppHandler<TestDto, Unit>
    {
        public Task<HandlerResult<Unit>> Handle(TestDto request)
        {
            var r = request.Property switch
            {
                eHandlerResult.Ok => HandlerResult<Unit>.Ok(new Unit()),
                eHandlerResult.NotFound => HandlerResult<Unit>.NotFound("Oops"),
                eHandlerResult.Rejected => HandlerResult<Unit>.Rejected(TestDto.ErrorMessage),
                _ => throw new ArgumentException($"Invalid property passed {request.Property}"),
            };

            return Task.FromResult(r);
        }

        public ValidationResult Validate(TestDto request)
        {
            var errors = new List<string>();
            if(request.Property == eHandlerResult.Invalid)
            {
                errors.Add(TestDto.ErrorMessage);
            }

            return new ValidationResult
            {
                Errors = errors.Select(x => new ValidationFailure
                {
                    ErrorMessage = x,
                }).ToList()
            };
        }
    }
}
