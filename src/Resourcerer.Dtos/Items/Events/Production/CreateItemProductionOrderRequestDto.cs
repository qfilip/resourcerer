namespace Resourcerer.Dtos;
public class CreateItemProductionOrderRequestDto : ItemProductionEventBaseDto
{
    public Guid ItemId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public DateTime DesiredProductionStartTime { get; set; }
    public Dictionary<Guid, double> InstancesToUse { get; set; } = [];
}
