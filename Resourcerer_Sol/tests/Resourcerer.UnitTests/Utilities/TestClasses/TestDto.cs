using FluentValidation;
using Resourcerer.Dtos;

namespace Resourcerer.UnitTests.Utilities.TestClasses;

public class TestDto : BaseDto
{
    public eHandlerResult Property { get; set; }
}

public class TestDtoValidator : AbstractValidator<TestDto>
{
    public const string ErrorMessage = "Test property must not be Invalid";

    public TestDtoValidator()
    {
        RuleFor(x => x.Property)
            .Equal(eHandlerResult.Invalid)
            .WithMessage(ErrorMessage);
    }
}
