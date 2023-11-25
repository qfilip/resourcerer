using FluentValidation;

namespace Resourcerer.Dtos;

public class CreateCompositeItemDto : IBaseDto
{
    public string? Name { get; set; }
    public double PreparationTimeSeconds { get; set; }
    public double? ExpirationTimeSeconds { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public double UnitPrice { get; set; }
    public Dictionary<Guid, double>? ExcerptMap { get; set; }

    public class Validator : AbstractValidator<CreateCompositeItemDto>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Element name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Element name must be between 3 and 50 characters long");

            RuleFor(x => x.PreparationTimeSeconds)
                .LessThan(0).WithMessage("PreparationTimeSeconds cannot be negative");

            RuleFor(x => x.ExpirationTimeSeconds)
                .Must(x =>
                {
                    if (x == null) return true;
                    else return x < 0;
                }).WithMessage("ExpirationTimeSeconds cannot be negative");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Element's category cannot be empty");

            RuleFor(x => x.UnitOfMeasureId)
                .NotEmpty().WithMessage("Element's unit of measure cannot be empty");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Element's price must be greater than 0");

            RuleFor(x => x.ExcerptMap)
                .Must(x =>
                {
                    if (x == null) return false;
                    else return x.All(kv => kv.Key != Guid.Empty && kv.Value >= 0);
                })
                .WithMessage("Excerpt map contains invalid data");
        }
    }
}
