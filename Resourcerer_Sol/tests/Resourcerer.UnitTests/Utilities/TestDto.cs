using FluentValidation;
using Resourcerer.Dtos;

namespace Resourcerer.UnitTests.Utilities;

public class TestDto : BaseDto
{
    public int Property { get; set; }
}

public class TestDtoValidator : AbstractValidator<TestDto>
{
    public const string ErrorMessage = "Test property must 0";
    
    public TestDtoValidator()
    {
        RuleFor(x => x.Property)
            .Equal(0)
            .WithMessage(ErrorMessage);
    }
}
