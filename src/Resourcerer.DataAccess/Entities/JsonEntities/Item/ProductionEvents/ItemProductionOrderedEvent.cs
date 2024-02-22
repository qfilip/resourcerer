using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class ItemProductionOrderedEvent : JsonEntityBase
{
    public double Quantity { get; set; }
    public ItemProductionOrderCancelledEvent? CanceledEvent { get; set; }
    public ItemProductionStartedEvent? StartedEvent { get; set; }
    public ItemProductionFailedEvent? FailedEvent { get; set; }
    public ItemProductionFinishedEvent? FinishedEvent { get; set; }
}
