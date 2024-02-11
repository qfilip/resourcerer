using FluentValidation;
using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class InstanceDeliveredRequestDto : InstanceEventDtoBase
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
