﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Functions.V1_0;

public static partial class Instances
{
    public static InstanceInfoDto GetInstanceInfo(Instance i, DateTime now)
    {
        var soldEvents = i.OrderedEvents
            .Where(x => x.InstanceOrderCancelledEvent == null)
            .ToArray();

        var sold = soldEvents.Sum(x => x.Quantity);

        var sellCancellationsPenaltyDifference = i.OrderedEvents
            .Where(x => x.InstanceOrderCancelledEvent != null)
            .Select(x => new
            {
                RefundedAmount = x.InstanceOrderCancelledEvent!.RefundedAmount,
                UnitPrice = x.UnitPrice,
                Quantity = x.Quantity
            })
            .Sum(x => x.RefundedAmount - (x.UnitPrice * x.Quantity));

        var sellProfits = soldEvents
            .Sum(x => Maths.Discount(x.Quantity * x.UnitPrice, x.TotalDiscountPercent));

        var discards = i.DiscardedEvents
            .Select(x => new DiscardInfoDto
            {
                Quantity = x.Quantity,
                Reason = x.Reason
            })
            .ToArray();

        var quantityLeft = i.Quantity - sold - discards.Sum(x => x.Quantity);

        return new InstanceInfoDto()
        {
            InstanceId = i.Id,
            PendingToArrive = 0,
            // PurchaseCost = i.UnitPurchaseCost * i.Quantity,
            Discards = discards,
            ExpiryDate = i.ExpiryDate,
            QuantityLeft = quantityLeft,
            SellProfit = sellProfits,
            SellCancellationsPenaltyDifference = (float)sellCancellationsPenaltyDifference
        };
    }
}
