namespace Resourcerer.Dtos.Entity;

public class UnitOfMeasureDto : EntityDto
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    // relational
    public Guid CompanyId { get; set; }
    public CompanyDto? Company { get; set; }

    public ItemDto[] Items { get; set; } = Array.Empty<ItemDto>();
}
