using FluentValidation;

namespace Resourcerer.Dtos;

public class CreateElementDto
{
    public string? Name { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public double UnitPrice { get; set; }
}

public class CreateElementDtoValidator : AbstractValidator<CreateElementDto>
{
    public CreateElementDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Element name cannot be empty")
            .Length(min: 3, max: 50).WithMessage("Element name must be between 3 and 50 characters long");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Element's category cannot be empty");

        RuleFor(x => x.UnitOfMeasureId)
            .NotEmpty().WithMessage("Element's unit of measure cannot be empty");

        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Element's price must be greater than 0");
    }
}
