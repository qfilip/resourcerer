namespace Resourcerer.DataAccess.Entities;

public class InstanceSellRequestedEvent : EntityBase
{
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    public Guid? InstanceRequestCancelledEventId { get; set; }
    public virtual InstanceRequestCancelledEvent? InstanceRequestCancelledEvent { get; set; }

    public Guid? InstanceRequestDeliveredEventId { get; set; }
    public virtual InstanceRequestDeliveredEvent? InstanceRequestDeliveredEvent { get; set; }
}
