namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderSendCommand : V1InstanceOrderCommand
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
