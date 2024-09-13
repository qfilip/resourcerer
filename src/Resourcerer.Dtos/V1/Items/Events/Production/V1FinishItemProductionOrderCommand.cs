namespace Resourcerer.Dtos.V1;

public class V1FinishItemProductionOrderCommand : V1ItemProductionCommand
{
    public Guid ProductionOrderId { get; set; }
}
