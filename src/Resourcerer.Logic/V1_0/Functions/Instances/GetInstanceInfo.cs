using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Functions.V1_0;

public static partial class Instances
{
    //public static InstanceInfoDto GetInstanceInfo(Instance i, string owner, DateTime now)
    //{
    //    if (i.ItemOrderedEvent!.ItemOrderCancelledEvent != null)
    //    {
    //        return MapCancelled(i);
    //    }

    //    if (i.ItemOrderedEvent!.ItemDeliveredEvent == null)
    //    {
    //        return MapNotDelivered(i);
    //    }

    //    var soldEvents = i.ItemSoldEvents
    //        .Where(x => x.ItemSellCancelledEvent == null)
    //        .ToArray();

    //    var sold = soldEvents
    //        .Sum(x => x.Quantity);

    //    var sellCancellationsPenaltyDifference = i.ItemSoldEvents
    //        .Where(x => x.ItemSellCancelledEvent != null)
    //        .Sum(x => x.ItemSellCancelledEvent!.RefundedAmount - (x.UnitPrice * x.Quantity));

    //    var sellProfits = soldEvents
    //        .Sum(x => Maths.Discount(x.Quantity * x.UnitPrice, x.TotalDiscountPercent));

    //    var discards = i.ItemDiscardedEvents
    //        .Select(x => new DiscardInfoDto
    //        {
    //            Quantity = x.Quantity,
    //            Reason = x.Reason
    //        })
    //        .ToArray();

    //    var quantityLeft = i.ItemOrderedEvent.Quantity - sold - discards.Sum(x => x.Quantity);
        
    //    return new InstanceInfoDto()
    //    {
    //        InstanceId = i.Id,
    //        PendingToArrive = 0,
    //        PurchaseCost = i.ItemOrderedEvent.UnitPrice * i.ItemOrderedEvent.Quantity,
    //        Discards = discards,
    //        ExpiryDate = i.ExpiryDate,
    //        QuantityLeft = quantityLeft,
    //        SellProfit = sellProfits,
    //        SellCancellationsPenaltyDifference = (float)sellCancellationsPenaltyDifference
    //    };
    //}

    //private static InstanceInfoDto MapNotDelivered(Instance i)
    //{
    //    return new InstanceInfoDto()
    //    {
    //        InstanceId = i.Id,
    //        PendingToArrive = i.ItemOrderedEvent!.Quantity,
    //        PurchaseCost = i.ItemOrderedEvent.UnitPrice * i.ItemOrderedEvent.Quantity
    //    };
    //}

    //private static InstanceInfoDto MapCancelled(Instance i)
    //{
    //    var buyEvent = i.ItemOrderedEvent!;
    //    var cancelEvent = i.ItemOrderedEvent!.ItemOrderCancelledEvent!;

    //    return new InstanceInfoDto()
    //    {
    //        InstanceId = i.Id,
    //        PendingToArrive = 0,
    //        PurchaseCost = (buyEvent.UnitPrice * buyEvent.Quantity) - cancelEvent.RefundedAmount
    //    };
    //}
}
