namespace Resourcerer.DataAccess.Entities;

public class ElementDiscardedEvent : EntityBase
{
    public Guid ElementInstanceId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
