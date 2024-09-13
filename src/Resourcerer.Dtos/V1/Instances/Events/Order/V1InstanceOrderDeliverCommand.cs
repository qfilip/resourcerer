namespace Resourcerer.Dtos.V1;

public class V1InstanceOrderDeliverCommand : V1InstanceOrderCommand
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
