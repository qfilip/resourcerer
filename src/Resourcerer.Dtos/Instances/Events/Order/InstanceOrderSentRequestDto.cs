namespace Resourcerer.Dtos.Instances.Events.Order;

public class InstanceOrderSentRequestDto : InstanceOrderEventDtoBase
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
