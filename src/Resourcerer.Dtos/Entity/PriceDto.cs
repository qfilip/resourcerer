namespace Resourcerer.Dtos.Entity;

public class PriceDto : IDto
{
    public double UnitValue { get; set; }

    public Guid? ElementId { get; set; }
    public ItemDto? Element { get; set; }
}


