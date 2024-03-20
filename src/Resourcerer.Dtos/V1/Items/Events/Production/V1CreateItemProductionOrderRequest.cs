namespace Resourcerer.Dtos.V1;
public class V1CreateItemProductionOrderRequest : V1ItemProductionEvent
{
    public Guid ItemId { get; set; }
    public Guid CompanyId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public DateTime DesiredProductionStartTime { get; set; }
    public Dictionary<Guid, double> InstancesToUse { get; set; } = [];
}
