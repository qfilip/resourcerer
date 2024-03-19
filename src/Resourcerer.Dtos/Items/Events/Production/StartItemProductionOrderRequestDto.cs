namespace Resourcerer.Dtos;

public class StartItemProductionOrderRequestDto : ItemProductionEventBaseDto
{
    public Guid ProductionOrderId { get; set; }
}
