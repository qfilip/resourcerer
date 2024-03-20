namespace Resourcerer.DataAccess.Entities;

public class InstanceDiscardedEvent : AppDbEntity
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }
}
