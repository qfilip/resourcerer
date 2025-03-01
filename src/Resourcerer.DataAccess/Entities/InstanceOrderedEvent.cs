using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : IId<Guid>, IAuditedEntity<Audit>
{
    public Guid DerivedInstanceId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public Guid? DerivedInstanceItemId { get; set; }
    public double Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    // json
    public InstanceOrderCancelledEvent? CancelledEvent { get; set; }
    public InstanceOrderSentEvent? SentEvent { get; set; }
    public InstanceOrderDeliveredEvent? DeliveredEvent { get; set; }

    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }

    // entity definition
    public Guid Id { get; set; }
    public Audit AuditRecord { get; set; } = new();
}
