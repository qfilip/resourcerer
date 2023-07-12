using Resourcerer.DataAccess.Enums;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : EntityBase
{
    public eOrderType OrderType { get; set; }
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? InstanceOrderCancelledEventId { get; set; }
    public virtual InstanceOrderCancelledEvent? InstanceOrderCancelledEvent { get; set; }

    public Guid? InstanceDeliveredEventId { get; set; }
    public virtual InstanceOrderDeliveredEvent? InstanceOrderDeliveredEvent { get; set; }
}
