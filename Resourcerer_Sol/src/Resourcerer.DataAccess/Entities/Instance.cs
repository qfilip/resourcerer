﻿namespace Resourcerer.DataAccess.Entities;

public class Instance : EntityBase
{
    public Instance()
    {
        ElementInstanceSoldEvents = new HashSet<ElementInstanceSoldEvent>();
        ElementInstanceDiscardedEvents = new HashSet<ElementInstanceDiscardedEvent>();
    }
    public double Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public Guid? ElementId { get; set; }
    public virtual Element? Element { get; set; }
    public Guid? CompositeId { get; set; }
    public virtual Composite? Composite { get; set; }

    public ICollection<ElementInstanceSoldEvent> ElementInstanceSoldEvents { get; set; }
    public ICollection<ElementInstanceDiscardedEvent> ElementInstanceDiscardedEvents { get; set; }
}
