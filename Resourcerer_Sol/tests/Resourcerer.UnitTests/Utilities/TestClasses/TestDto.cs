using FluentValidation;
using Resourcerer.Dtos;

namespace Resourcerer.UnitTests.Utilities.TestClasses;

public class TestDto : IBaseDto<TestDto>
{
    public eHandlerResult Property { get; set; }

    public AbstractValidator<TestDto>? GetValidator() => new Validator();

    public const string ErrorMessage = "Test property must not be Invalid";
    private class Validator : AbstractValidator<TestDto>
    {

        public Validator()
        {
            RuleFor(x => x.Property)
                .Must(e => e != eHandlerResult.Invalid)
                .WithMessage(ErrorMessage);
        }
    }
}
