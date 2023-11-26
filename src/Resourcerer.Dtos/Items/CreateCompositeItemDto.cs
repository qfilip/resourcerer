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
}
