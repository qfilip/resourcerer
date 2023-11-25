﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Functions;

public static partial class Instances
{
    public static InstanceInfoDto GetInstanceInfo(Instance i, DateTime now)
    {
        if (i.InstanceBoughtEvent!.InstanceCancelledEvent != null)
        {
            return MapCancelled(i);
        }

        if (i.InstanceBoughtEvent!.InstanceDeliveredEvent == null)
        {
            return MapNotDelivered(i);
        }

        var soldEvents = i.InstanceSoldEvents
            .Where(x => x.InstanceCancelledEvent == null)
            .ToArray();

        var sold = soldEvents
            .Sum(x => x.Quantity);

        var sellCancellationsPenaltyDifference = i.InstanceSoldEvents
            .Where(x => x.InstanceCancelledEvent != null)
            .Sum(x => x.InstanceCancelledEvent!.RefundedAmount - (x.UnitPrice * x.Quantity));

        var sellProfits = soldEvents
            .Sum(x => Maths.Discount(x.Quantity * x.UnitPrice, x.TotalDiscountPercent));

        var discards = i.InstanceDiscardedEvents
            .Select(x => new DiscardInfoDto
            {
                Quantity = x.Quantity,
                Reason = x.Reason
            })
            .ToArray();

        var quantityLeft = i.InstanceBoughtEvent.Quantity - sold - discards.Sum(x => x.Quantity);
        
        if(i.ExpiryDate < now)
        {
            quantityLeft = i.InstanceBoughtEvent!.Quantity - sold - discards.Sum(x => x.Quantity);
        }

        return new InstanceInfoDto()
        {
            InstanceId = i.Id,
            PendingToArrive = 0,
            PurchaseCost = i.InstanceBoughtEvent.UnitPrice * i.InstanceBoughtEvent.Quantity,
            Discards = discards,
            ExpiryDate = i.ExpiryDate,
            QuantityLeft = quantityLeft,
            SellProfit = sellProfits,
            SellCancellationsPenaltyDifference = (float)sellCancellationsPenaltyDifference
        };
    }

    private static InstanceInfoDto MapNotDelivered(Instance i)
    {
        return new InstanceInfoDto()
        {
            InstanceId = i.Id,
            PendingToArrive = i.InstanceBoughtEvent!.Quantity,
            PurchaseCost = i.InstanceBoughtEvent.UnitPrice * i.InstanceBoughtEvent.Quantity
        };
    }

    private static InstanceInfoDto MapCancelled(Instance i)
    {
        var buyEvent = i.InstanceBoughtEvent!;
        var cancelEvent = i.InstanceBoughtEvent!.InstanceCancelledEvent!;

        return new InstanceInfoDto()
        {
            InstanceId = i.Id,
            PendingToArrive = 0,
            PurchaseCost = (buyEvent.UnitPrice * buyEvent.Quantity) - cancelEvent.RefundedAmount
        };
    }
}
