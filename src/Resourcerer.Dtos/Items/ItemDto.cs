using FluentValidation;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos;

public class ItemDto : BaseDto<ItemDto>
{
    public string? Name { get; set; }

    public Guid? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasureDto? UnitOfMeasure { get; set; }

    public List<ExcerptDto>? Excerpts { get; set; }
    public List<Price>? Prices { get; set; }

    public override AbstractValidator<ItemDto> GetValidator() => new Validator();

    public class Validator : AbstractValidator<ItemDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Element name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Element name must be between 3 and 50 characters long");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Element's category cannot be empty");

            RuleFor(x => x.UnitOfMeasureId)
                .NotEmpty().WithMessage("Element's unit of measure cannot be empty");
        }
    }
}


