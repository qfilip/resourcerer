using FluentValidation;
using Resourcerer.Dtos;

namespace Resourcerer.UnitTests.Utilities.TestClasses;

public class TestDto : IBaseDto
{
    public eHandlerResult Property { get; set; }
    public const string ErrorMessage = "Test property must not be Invalid";
    
    public class Validator : AbstractValidator<TestDto>
    {
        public Validator()
        {
            RuleFor(x => x.Property)
                .Must(e => e != eHandlerResult.Invalid)
                .WithMessage(ErrorMessage);
        }
    }
}
