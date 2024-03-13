using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrderCancelledEvent : JsonEntityBase
{
    public string? Reason { get; set; }
}
