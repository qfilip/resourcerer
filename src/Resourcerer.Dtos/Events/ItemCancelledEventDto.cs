using Resourcerer.Dtos.Events;

namespace Resourcerer.Dtos;

public class ItemCancelledEventDto : ItemEventDtoBase
{
    public Guid TargetEventId { get; set; }
    public string? Reason { get; set; }
    public double RefundedAmount { get; set; }
}
