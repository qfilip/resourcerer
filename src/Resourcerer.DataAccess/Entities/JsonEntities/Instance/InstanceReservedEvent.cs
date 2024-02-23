using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class InstanceReservedEvent : JsonEntityBase
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
