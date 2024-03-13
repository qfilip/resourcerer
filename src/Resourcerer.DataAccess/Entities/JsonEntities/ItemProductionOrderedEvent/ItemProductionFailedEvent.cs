using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionFailedEvent : JsonEntityBase
{
    public string? Reason { get; set; }
}
