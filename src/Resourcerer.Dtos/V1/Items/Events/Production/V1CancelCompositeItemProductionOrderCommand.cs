namespace Resourcerer.Dtos.V1;

public class V1CancelCompositeItemProductionOrderCommand : V1ItemProductionCommand
{
    public Guid ProductionOrderId { get; set; }
    public string? Reason { get; set; }
}
