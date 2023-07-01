using FluentValidation;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Categories;
using Resourcerer.Dtos.Excerpts;
using Resourcerer.Dtos.UnitsOfMeasure;

namespace Resourcerer.Dtos.Elements;

public class ElementDto : EntityDto
{
    public string? Name { get; set; }
    public double CurrentSellPrice { get; set; }

    public Guid? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasureDto? UnitOfMeasure { get; set; }

    public List<ExcerptDto>? Excerpts { get; set; }
    public List<OldPrice>? OldPrices { get; set; }
    public List<ElementPurchasedEvent>? ElementPurchasedEvents { get; set; }
    public List<ElementSoldEvent>? ElementSoldEvents { get; set; }
}

public class ElementDtoValidator : AbstractValidator<ElementDto>
{
    public ElementDtoValidator()
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
