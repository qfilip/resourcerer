using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.Entities.Json;

namespace Resourcerer.Dtos;

public class ItemProductionOrderDto : EntityDto
{
    public double Quantity { get; set; }
    public string? Reason { get; set; }
    public Guid CompanyId { get; set; }
    public int ItemRecipeVersion { get; set; }

    // relational
    public Guid ItemId { get; set; }
    public ItemDto? Item { get; set; }

    // json
    public Guid[] InstancesUsedIds { get; set; } = Array.Empty<Guid>();
    public ItemProductionStartedEventDto? StartedEvent { get; set; }
    public ItemProductionOrderCancelledEventDto? CancelledEvent { get; set; }
    public ItemProductionFinishedEventDto? FinishedEvent { get; set; }
}
