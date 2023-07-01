using FluentValidation;
using Resourcerer.Dtos.Categories;
using Resourcerer.Dtos.CompositeSoldEvents;
using Resourcerer.Dtos.Excerpts;
using Resourcerer.Dtos.OldPrices;

namespace Resourcerer.Dtos.Composites;

public class CompositeDto : EntityDto
{
    public string? Name { get; set; }
    public double CurrentSellPrice { get; set; }
    
    public Guid CategoryId { get; set; }

    public CategoryDto? Category { get; set; }
    public List<OldPriceDto> OldPrices { get; set; } = new();
    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<CompositeSoldEventDto> CompositeSoldEvents { get; set; } = new();
}

public class CompositeDtoValidator : AbstractValidator<CompositeDto>
{
    public CompositeDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Composite name cannot be empty");

        RuleFor(x => x.CurrentSellPrice)
            .GreaterThan(0)
            .WithMessage("Composite sell price must be larger than 0");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Composite category must be set");
    }
}
