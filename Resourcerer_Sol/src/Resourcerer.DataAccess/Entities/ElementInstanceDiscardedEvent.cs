namespace Resourcerer.DataAccess.Entities;

public class ElementInstanceDiscardedEvent : EntityBase
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    
    public Guid ElementInstanceId { get; set; }
    public virtual Instance? Instance { get; set; }
}
