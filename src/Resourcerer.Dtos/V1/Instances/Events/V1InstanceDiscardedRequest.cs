namespace Resourcerer.Dtos.V1;

public class V1InstanceDiscardedRequest
{
    public Guid InstanceId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
