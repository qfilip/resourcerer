namespace Resourcerer.Dtos.V1;

public class V1CancelItemProductionOrderRequest : V1ItemProductionEvent
{
    public Guid ProductionOrderEventId { get; set; }
    public string? Reason { get; set; }
}
