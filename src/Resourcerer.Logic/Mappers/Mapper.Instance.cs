using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static Instance Map(InstanceDto src) =>
        Map(() =>
            new Instance
            {
                Quantity = src.Quantity,
                ExpiryDate = src.ExpiryDate,

                ItemId = src.ItemId,
                Item = Map(src.Item, Map),

                OwnerCompanyId = src.OwnerCompanyId,
                OwnerCompany = Map(src.OwnerCompany, Map),

                SourceInstanceId = src.SourceInstanceId,
                SourceInstance = Map(src.SourceInstance, Map),

                DerivedInstances = src.DerivedInstances.Select(x => Map(x)).ToArray(),

                OrderedEvents = src.OrderedEvents.Select(x => Map(x)).ToArray(),
                ReservedEvents = src.ReservedEvents.Select(x => Map(x)).ToArray(),
                DiscardedEvents = src.DiscardedEvents.Select(x => Map(x)).ToArray()
            }, src);

    public static InstanceDto Map(Instance src) =>
        Map(() =>
            new InstanceDto
            {
                Quantity = src.Quantity,
                ExpiryDate = src.ExpiryDate,

                ItemId = src.ItemId,
                Item = Map(src.Item, Map),

                OwnerCompanyId = src.OwnerCompanyId,
                OwnerCompany = Map(src.OwnerCompany, Map),

                SourceInstanceId = src.SourceInstanceId,
                SourceInstance = Map(src.SourceInstance, Map),

                DerivedInstances = src.DerivedInstances.Select(x => Map(x)).ToArray(),

                OrderedEvents = src.OrderedEvents.Select(x => Map(x)).ToArray(),
                ReservedEvents = src.ReservedEvents.Select(x => Map(x)).ToArray(),
                DiscardedEvents = src.DiscardedEvents.Select(x => Map(x)).ToArray()
            }, src);
}
