namespace Resourcerer.Dtos;

public class FinishItemProductionOrderRequest : ItemProductionEventBaseDto
{
    public Guid ProductionOrderId { get; set; }
}
