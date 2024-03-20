namespace Resourcerer.Dtos.V1;

public class V1StartItemProductionOrderRequest : V1ItemProductionEvent
{
    public Guid ProductionOrderId { get; set; }
}
