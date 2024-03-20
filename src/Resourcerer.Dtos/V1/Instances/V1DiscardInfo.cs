namespace Resourcerer.Dtos.V1;

public class V1DiscardInfo : IDto
{
    public string? Reason { get; set; }
    public double Quantity { get; set; }
}
