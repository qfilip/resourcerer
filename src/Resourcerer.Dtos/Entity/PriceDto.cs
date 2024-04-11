using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Dtos.Entity;

public class PriceDto : EntityDto<PriceDto>
{
    public double UnitValue { get; set; }

    public Guid? ItemId { get; set; }
    public ItemDto? Item { get; set; }
}


