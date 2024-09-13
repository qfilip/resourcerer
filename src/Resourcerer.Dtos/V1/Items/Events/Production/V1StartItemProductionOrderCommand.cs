namespace Resourcerer.Dtos.V1;

public class V1StartItemProductionOrderCommand : V1ItemProductionCommand
{
    public Guid ProductionOrderId { get; set; }
}
