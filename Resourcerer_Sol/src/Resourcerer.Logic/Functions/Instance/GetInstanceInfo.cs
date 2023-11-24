using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Instances;
using Resourcerer.Utilities;

namespace Resourcerer.Logic.Functions;

public static partial class Instances
{
    public static InstanceInfoDto? GetInstanceInfo(Instance i, DateTime now)
    {
        if (i.InstanceBoughtEvent!.InstanceDeliveredEvent == null)
        {
            return null;
        }

        var soldEvents = i.InstanceSoldEvents
            .Where(x => x.InstanceCancelledEvent == null)
            .ToArray();

        var sold = soldEvents
            .Sum(x => x.Quantity);

        var sellProfits = soldEvents
            .Sum(x => Maths.Discount(x.Quantity * x.UnitPrice, x.TotalDiscountPercent));

        var discards = i.InstanceDiscardedEvents
            .Select(x => new DiscardInfoDto
            {
                Quantity = x.Quantity,
                Reason = x.Reason
            })
            .ToArray();

        var quantityLeft = 0d;
        
        if(i.ExpiryDate < now)
        {
            quantityLeft = i.InstanceBoughtEvent!.Quantity - sold - discards.Sum(x => x.Quantity);
        }

        return new InstanceInfoDto
        {
            InstanceId = i.Id,
            Discards = discards,
            ExpiryDate = i.ExpiryDate,
            PurchaseCost = i.InstanceBoughtEvent!.UnitPrice,
            QuantityLeft = quantityLeft,
            SellProfit = sellProfits
        };
    }
}
