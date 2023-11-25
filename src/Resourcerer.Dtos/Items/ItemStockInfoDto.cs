namespace Resourcerer.Dtos.Items;

public class ItemStockInfoDto : IBaseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public double TotalUnitsInStock { get; set; }
    public InstanceInfoDto[]? InstancesInfo { get; set; }
    public string[]? ItemType { get; set; }
    public double ProductionCostAsComposite { get; set; }
    public double SellingPrice { get; set; }
}
