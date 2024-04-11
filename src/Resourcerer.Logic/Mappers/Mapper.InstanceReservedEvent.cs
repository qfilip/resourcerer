using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static InstanceReservedEvent Map(InstanceReservedEventDto src) =>
        Map(() =>
            new InstanceReservedEvent
            {
                ItemProductionOrderId = src.ItemProductionOrderId,
                Quantity = src.Quantity,
                Reason = src.Reason,

                InstanceId = src.InstanceId,
                Instance = Map(src.Instance, Map),

                CancelledEvent = src.CancelledEvent,
                UsedEvent = src.UsedEvent
        }, src);

    public static InstanceReservedEventDto Map(InstanceReservedEvent src) =>
        Map(() =>
            new InstanceReservedEventDto
            {
                ItemProductionOrderId = src.ItemProductionOrderId,
                Quantity = src.Quantity,
                Reason = src.Reason,

                InstanceId = src.InstanceId,
                Instance = Map(src.Instance, Map),

                CancelledEvent = src.CancelledEvent,
                UsedEvent = src.UsedEvent
            }, src);
}
