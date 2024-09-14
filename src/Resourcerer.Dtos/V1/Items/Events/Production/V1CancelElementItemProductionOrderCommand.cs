namespace Resourcerer.Dtos.V1;

public sealed class V1CancelElementItemProductionOrderCommand : V1ItemProductionCommand
{
    public Guid ProductionOrderId { get; set; }
    public string? Reason { get; set; }
}
