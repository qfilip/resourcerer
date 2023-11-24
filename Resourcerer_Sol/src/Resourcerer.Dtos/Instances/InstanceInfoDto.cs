using Resourcerer.Dtos.Instances;

namespace Resourcerer.Dtos;

public class InstanceInfoDto : IBaseDto
{
    public Guid InstanceId { get; set; }
    public double PendingToArrive { get; set; }
    public double QuantityLeft { get; set; }
    public DiscardInfoDto[]? Discards { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public double PurchaseCost { get; set; }
    public double SellProfit { get; set; }
}
