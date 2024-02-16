﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Functions.V1_0;

public static partial class Instances
{
    public static double GetUnitsInStock(Instance i)
    {
        if (i.SourceInstanceId == null)
        {
            return i.Quantity;
        }

        if(i.SourceInstance == null)
        {
            throw new InvalidOperationException($"Source instance for instance {i.Id} is null");
        }

        var completedOrderEvent = i.SourceInstance.OrderedEvents
            .Where(x =>
                x.DerivedInstanceId == i.Id &&
                x.DeliveredEvent != null)
            .FirstOrDefault();

        return completedOrderEvent == null ? 0 : completedOrderEvent.Quantity;
    }
    public static InstanceInfoDto GetInstanceInfo(Instance i, DateTime now)
    {
        var soldEvents = i.OrderedEvents
            .Where(x => x.OrderCancelledEvent == null)
            .ToArray();

        var sold = soldEvents.Sum(x => x.Quantity);

        var sellCancellationsPenaltyDifference = i.OrderedEvents
            .Where(x => x.OrderCancelledEvent != null)
            .Select(x => new
            {
                RefundedAmount = x.OrderCancelledEvent!.RefundedAmount,
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