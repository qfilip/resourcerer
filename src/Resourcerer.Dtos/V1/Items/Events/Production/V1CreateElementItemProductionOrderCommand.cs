namespace Resourcerer.Dtos.V1;

public sealed class V1CreateElementItemProductionOrderCommand : V1ItemProductionCommand
{
    public Guid ItemId { get; set; }
    public Guid CompanyId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public bool InstantProduction { get; set; }
    public DateTime DesiredProductionStartTime { get; set; }
}
