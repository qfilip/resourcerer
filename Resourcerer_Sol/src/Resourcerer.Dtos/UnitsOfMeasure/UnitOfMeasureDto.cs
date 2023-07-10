﻿using FluentValidation;

namespace Resourcerer.Dtos;

public class UnitOfMeasureDto : EntityDto<UnitOfMeasureDto>
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<ElementDto> Elements { get; set; } = new();

    public override AbstractValidator<UnitOfMeasureDto> GetValidator() => new Validator();

    public class Validator : AbstractValidator<UnitOfMeasureDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Unit of measure name cannot be empty")
                .Length(min: 2, max: 50).WithMessage("Unit of measure name must be between 2 and 50 characters long");

            RuleFor(x => x.Symbol)
                .NotEmpty().WithMessage("Unit of measure symbol cannot be empty")
                .Length(min: 1, max: 12).WithMessage("Unit of measure symbol must be between 1 and 12 characters long");
        }
    }
}
