using Resourcerer.Dtos.Instances;

namespace Resourcerer.Dtos;

public class InstanceInfoDto
{
    public Guid InstanceId { get; set; }
    public Guid ItemId { get; set; }
    public string? ItemName { get; set; }

    public double QuantityLeft { get; set; }
    public List<DiscardInfo> Discards { get; set; } = new();
    public DateTime ExpiryDate { get; set; }
    public double PurchaseCost { get; set; }
    public double SellProfit { get; set; }
}
