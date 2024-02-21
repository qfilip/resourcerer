using FluentValidation;

namespace Resourcerer.Dtos.Instances.Events.Order;

public class InstanceOrderDeliveredRequestDto : InstanceOrderEventDtoBase
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
