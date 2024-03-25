using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.V1.Functions;

public static partial class Instances
{
    public static V1InstanceInfo GetInstanceInfo(Instance i, DateTime now)
    {
        var soldEvents = i.OrderedEvents
            .Where(x => x.CancelledEvent == null)
            .ToArray();

        var sold = soldEvents.Sum(x => x.Quantity);

        var sellCancellationsPenaltyDifference = i.OrderedEvents
            .Where(x => x.CancelledEvent != null)
            .Select(x => new
            {
                RefundedAmount = x.CancelledEvent!.RefundedAmount,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            })
            .Sum(x => x.RefundedAmount - (x.UnitPrice * x.Quantity));

        var sellProfits = soldEvents
            .Sum(x => Maths.Discount(x.Quantity * x.UnitPrice, x.TotalDiscountPercent));

        var discards = i.DiscardedEvents
            .Select(x => new V1DiscardInfo
            {
                Quantity = x.Quantity,
                Reason = x.Reason
            })
            .ToArray();

        var quantityLeft = i.Quantity - sold - discards.Sum(x => x.Quantity);
        var purchaseCost = ComputePurchaseCost(i);

        return new V1InstanceInfo()
        {
            InstanceId = i.Id,
            PendingToArrive = 0,
            PurchaseCost = purchaseCost,
            Discards = discards,
            ExpiryDate = i.ExpiryDate,
            QuantityLeft = quantityLeft,
            SellProfit = sellProfits,
            SellCancellationsPenaltyDifference = (float)sellCancellationsPenaltyDifference
        };
    }

    private static double ComputePurchaseCost(Instance i)
    {
        if(!i.SourceInstanceId.HasValue)
        {
            return i.Item!.ProductionPrice;
        }

        var orderEvent = i.SourceInstance!.OrderedEvents
            .First(x => x.DerivedInstanceId == i.Id);

        return Maths.Discount(orderEvent.Quantity * orderEvent.UnitPrice, orderEvent.TotalDiscountPercent);
    }
}
