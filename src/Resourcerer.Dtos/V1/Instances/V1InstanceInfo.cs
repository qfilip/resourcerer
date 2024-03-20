namespace Resourcerer.Dtos.V1;

public class V1InstanceInfo : IDto
{
    public Guid InstanceId { get; set; }
    public double PendingToArrive { get; set; }
    public double QuantityLeft { get; set; }
    public V1DiscardInfo[]? Discards { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public double PurchaseCost { get; set; }
    public double SellProfit { get; set; }
    public float SellCancellationsPenaltyDifference { get; set; }
}
