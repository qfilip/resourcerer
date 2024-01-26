﻿namespace Resourcerer.DataAccess.Entities;

public class ItemDeliveredEvent : EntityBase
{
    public Guid? ItemOrderedEventId { get; set; }
    public virtual ItemOrderedEvent? ItemOrderedEvent { get; set; }
}
