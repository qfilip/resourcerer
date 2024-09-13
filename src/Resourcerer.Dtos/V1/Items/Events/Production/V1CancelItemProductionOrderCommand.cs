namespace Resourcerer.Dtos.V1;

public class V1CancelItemProductionOrderCommand : V1ItemProductionCommand
{
    public Guid ProductionOrderEventId { get; set; }
    public string? Reason { get; set; }
}
