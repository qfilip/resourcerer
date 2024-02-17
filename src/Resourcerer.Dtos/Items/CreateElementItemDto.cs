using FluentValidation;

namespace Resourcerer.Dtos;

public class CreateElementItemDto : IBaseDto
{
    public string? Name { get; set; }
    public double PreparationTimeSeconds { get; set; }
    public double? ExpirationTimeSeconds { get; set; }
    public Guid CompanyId { get; set; }
    public Guid CategoryId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public double UnitPrice { get; set; }

    
}
