﻿using FluentValidation;

namespace Resourcerer.Dtos.Composites;

public class CreateCompositeDto : BaseDto
{
    public string? Name { get; set; }
    public Guid CategoryId { get; set; }
    public double PriceByUnit { get; set; }
    public List<CompositeElementsDto>? Elements { get; set; }
}

public class CreateCompositeDtoValidator : AbstractValidator<CreateCompositeDto>
{
    public CreateCompositeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Composite name cannot be empty");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Composite category must be set");

        RuleFor(x => x.PriceByUnit)
            .GreaterThan(0)
            .WithMessage("Composite price cannot be 0");

        RuleFor(x => x.Elements)
            .NotNull()
            .WithMessage("Composite elements cannot be null")
            .NotEmpty()
            .WithMessage("Composite must be made of at least one element")
            .Must(x => x!.All(e => e.ElementId != Guid.Empty))
            .WithMessage("All element ids must have non-default uuid value")
            .Must(x => x!.All(e => e.Quantity > 0))
            .WithMessage("All elements must have quantity larger than zero");
    }
}
