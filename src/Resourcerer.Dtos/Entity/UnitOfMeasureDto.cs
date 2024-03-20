namespace Resourcerer.Dtos.Entity;

public class UnitOfMeasureDto : IDto
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<ItemDto> Elements { get; set; } = new();
}
