using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static ItemProductionOrder Map(ItemProductionOrderDto src) =>
        Map(() =>
            new ItemProductionOrder
            {
                Quantity = src.Quantity,
                Reason = src.Reason,
                CompanyId = src.CompanyId,

                ItemId = src.ItemId,
                Item = Map(src.Item, Map),

                InstancesUsedIds = src.InstancesUsedIds,

                StartedEvent = src.StartedEvent,
                CancelledEvent = src.CanceledEvent,
                FinishedEvent = src.FinishedEvent
            }, src);

    public static ItemProductionOrderDto Map(ItemProductionOrder src) =>
        Map(() =>
            new ItemProductionOrderDto
            {
                Quantity = src.Quantity,
                Reason = src.Reason,
                CompanyId = src.CompanyId,

                ItemId = src.ItemId,
                Item = Map(src.Item, Map),

                InstancesUsedIds = src.InstancesUsedIds,

                StartedEvent = src.StartedEvent,
                CanceledEvent = src.CancelledEvent,
                FinishedEvent = src.FinishedEvent
            }, src);
}
