namespace Resourcerer.Dtos.Instances.Events;

public class InstanceDiscardedRequestDto
{
    public Guid InstanceId { get; set; }
    public double Quantity { get; set; }
    public string? Reason { get; set; }
}
