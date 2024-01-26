﻿namespace Resourcerer.DataAccess.Entities;

public class ItemDiscardedEvent : EntityBase
{
    public string? Owner { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }

    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }
}
