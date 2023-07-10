﻿using FluentValidation;

namespace Resourcerer.Dtos;

public class CompositeDto : EntityDto<CompositeDto>
{
    public string? Name { get; set; }
    
    public Guid CategoryId { get; set; }

    public CategoryDto? Category { get; set; }
    public List<PriceDto> Prices { get; set; } = new();
    public List<ExcerptDto> Excerpts { get; set; } = new();

    public override AbstractValidator<CompositeDto> GetValidator() => new CompositeDtoValidator();
}

public class CompositeDtoValidator : AbstractValidator<CompositeDto>
{
    public CompositeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Composite name cannot be empty");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Composite category must be set");
    }
}
