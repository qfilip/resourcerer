﻿using Resourcerer.DataAccess.Entities.JsonEntities;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : JsonEntityBase
{
    public Guid DerivedInstanceId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    public virtual InstanceOrderCancelledEvent? OrderCancelledEvent { get; set; }
    public virtual InstanceSentEvent? SentEvent { get; set; }
    public virtual InstanceDeliveredEvent? DeliveredEvent { get; set; }
}
