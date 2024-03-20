using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace Resourcerer.DataAccess.Entities;

public class InstanceOrderedEvent : AppDbEntity
{
    public Guid DerivedInstanceId { get; set; }
    public Guid SellerCompanyId { get; set; }
    public Guid BuyerCompanyId { get; set; }
    public Guid? DerivedInstanceItemId { get; set; }
    public double Quantity { get; set; }
    public double UnitPrice { get; set; }
    public int TotalDiscountPercent { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }

    // json
    [NotMapped]
    public InstanceOrderCancelledEvent? CancelledEvent { get; set; }
    [NotMapped]
    public InstanceOrderSentEvent? SentEvent { get; set; }
    [NotMapped]
    public InstanceOrderDeliveredEvent? DeliveredEvent { get; set; }

    public string CancelledEventJson
    {
        get => JsonSerializer.Serialize(CancelledEvent);
        private set
        {
            if (value == null) return;
            CancelledEvent = JsonSerializer.Deserialize<InstanceOrderCancelledEvent>(value);
        }
    }
    public string SentEventJson
    {
        get => JsonSerializer.Serialize(SentEvent);
        private set
        {
            if (value == null) return;
            SentEvent = JsonSerializer.Deserialize<InstanceOrderSentEvent>(value);
        }
    }
    public string DeliveredEventJson
    {
        get => JsonSerializer.Serialize(DeliveredEvent);
        private set
        {
            if (value == null) return;
            DeliveredEvent = JsonSerializer.Deserialize<InstanceOrderDeliveredEvent>(value);
        }
    }
    // relational
    public Guid InstanceId { get; set; }
    public virtual Instance? Instance { get; set; }
}
