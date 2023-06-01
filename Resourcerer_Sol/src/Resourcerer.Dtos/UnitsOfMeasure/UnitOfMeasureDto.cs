using Resourcerer.Dtos.Elements;
using Resourcerer.Dtos.Excerpts;

namespace Resourcerer.Dtos.UnitsOfMeasure;

public class UnitOfMeasureDto : DtoBase
{
    public string? Name { get; set; }
    public string? Symbol { get; set; }

    public List<ExcerptDto> Excerpts { get; set; } = new();
    public List<ElementDto> Elements { get; set; } = new();
}
