namespace Resourcerer.Dtos;
public class CreateItemProductionOrderRequestDto : ItemProductionEventBaseDto
{
    public Guid ItemId { get; set; }
    public double Quantity { get; set; }
    public DateTime DesiredProductionStartTime { get; set; }
    public Guid[] InstanceToUseIds { get; set; } = Array.Empty<Guid>();
}
