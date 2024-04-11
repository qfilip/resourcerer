using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static InstanceDiscardedEvent Map(InstanceDiscardedEventDto src) =>
        Map(() =>
            new InstanceDiscardedEvent
            {
                Quantity = src.Quantity,
                Reason = src.Reason,
                
                InstanceId = src.InstanceId,
                Instance = Map(src.Instance, Map),
            }, src);

    public static InstanceDiscardedEventDto Map(InstanceDiscardedEvent src) =>
        Map(() =>
            new InstanceDiscardedEventDto
            {
                Quantity = src.Quantity,
                Reason = src.Reason,

                InstanceId = src.InstanceId,
                Instance = Map(src.Instance, Map),
            }, src);
}
