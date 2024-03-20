namespace Resourcerer.Dtos.V1;

public class V1ItemStockInfo : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double TotalUnitsInStock { get; set; }
    public V1InstanceInfo[]? InstancesInfo { get; set; }
    public string[]? ItemType { get; set; }
    public double ProductionCostAsComposite { get; set; }
    public double SellingPrice { get; set; }
}
