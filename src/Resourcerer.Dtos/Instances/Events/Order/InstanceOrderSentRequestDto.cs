namespace Resourcerer.Dtos.Instances.Events.Order;

public class InstanceOrderSentRequestDto : InstanceOrderEventDtoBase
{
    public Guid InstanceId { get; set; }
    public string? OrderEventId { get; set; }
}
