namespace Resourcerer.Dtos.V1;

public class V1FinishItemProductionOrderRequest : V1ItemProductionEvent
{
    public Guid ProductionOrderId { get; set; }
}
