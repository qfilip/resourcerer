using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrderCancelledEvent : AppDbJsonField
{
    public string? Reason { get; set; }
}
