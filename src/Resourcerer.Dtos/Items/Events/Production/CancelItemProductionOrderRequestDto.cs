namespace Resourcerer.Dtos;

public class CancelItemProductionOrderRequestDto : ItemProductionEventBaseDto
{
    public Guid ProductionOrderEventId { get; set; }
    public string? Reason { get; set; }
}
