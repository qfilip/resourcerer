namespace Resourcerer.Dtos.V1;

public class V1InstanceInfo : IDto
{
    public Guid InstanceId { get; set; }
    public double PendingToArrive { get; set; }
    public double QuantityLeft { get; set; }
    public V1DiscardInfo[]? Discards { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal PurchaseCost { get; set; }
    public decimal SellProfit { get; set; }
    public decimal SellCancellationsPenaltyDifference { get; set; }
}
