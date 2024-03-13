namespace Resourcerer.Dtos;

public class UnitOfMeasureDto : IBaseDto
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<ItemDto> Elements { get; set; } = new();
}
