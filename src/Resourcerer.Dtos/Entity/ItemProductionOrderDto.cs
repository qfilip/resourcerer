﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Dtos;

public class ItemProductionOrderDto : EntityDto
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public Guid CompanyId { get; set; }
    public int ItemRecipeVersion { get; set; }

    // relational
    public Guid ItemId { get; set; }
    public ItemDto? Item { get; set; }

    // json
    public Guid[] InstancesUsedIds { get; set; } = Array.Empty<Guid>();
    public ItemProductionStartedEvent? StartedEvent { get; set; }
    public ItemProductionOrderCancelledEvent? CancelledEvent { get; set; }
    public ItemProductionFinishedEvent? FinishedEvent { get; set; }
}
