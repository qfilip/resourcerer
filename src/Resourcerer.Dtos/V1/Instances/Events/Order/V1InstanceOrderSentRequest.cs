namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderSentRequest : V1InstanceOrderEvent
{
    public Guid InstanceId { get; set; }
    public string? OrderEventId { get; set; }
}
