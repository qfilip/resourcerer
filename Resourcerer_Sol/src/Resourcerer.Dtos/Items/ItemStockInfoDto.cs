namespace Resourcerer.Dtos.Items;

public class ItemStockInfoDto : IBaseDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public InstanceInfoDto[]? InstancesInfo { get; set; }
}
