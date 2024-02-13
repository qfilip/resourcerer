using FluentValidation;
using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class InstanceDeliveredRequestDto : EventDtoBase
{
    public Guid InstanceId { get; set; }
    public Guid OrderEventId { get; set; }
}
