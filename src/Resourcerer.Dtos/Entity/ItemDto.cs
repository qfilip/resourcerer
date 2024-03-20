using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos.Entity;

public class ItemDto : IDto
{
    public string? Name { get; set; }

    public Guid? CategoryId { get; set; }
    public CategoryDto? Category { get; set; }

    public Guid UnitOfMeasureId { get; set; }
    public UnitOfMeasureDto? UnitOfMeasure { get; set; }

    public List<ExcerptDto>? Excerpts { get; set; }
    public List<Price>? Prices { get; set; }
}


