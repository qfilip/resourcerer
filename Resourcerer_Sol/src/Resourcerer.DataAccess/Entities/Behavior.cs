namespace Resourcerer.DataAccess.Entities;

public class Behavior : EntityBase
{
    public TimeSpan? ExpirationTime { get; set; }

    public Guid? ElementId { get; set; }
    public virtual Element? Element { get; set; }

    public Guid? CompositeId { get; set; }
    public virtual Composite? Composite { get; set; }
}
