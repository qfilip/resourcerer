using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static InstanceOrderedEvent Map(InstanceOrderedEventDto src) =>
        Map(() =>
            new InstanceOrderedEvent
            {
                DerivedInstanceId = src.DerivedInstanceId,
                SellerCompanyId = src.SellerCompanyId,
                BuyerCompanyId = src.BuyerCompanyId,
                DerivedInstanceItemId = src.DerivedInstanceItemId,
                Quantity = src.Quantity,
                UnitPrice = src.UnitPrice,
                TotalDiscountPercent = src.TotalDiscountPercent,
                ExpectedDeliveryDate = src.ExpectedDeliveryDate,
                
                InstanceId = src.InstanceId,
                Instance = Map(src.Instance, Map),

                CancelledEvent = src.CancelledEvent,
                SentEvent = src.SentEvent,
                DeliveredEvent = src.DeliveredEvent
            }, src);

    public static InstanceOrderedEventDto Map(InstanceOrderedEvent src) =>
        Map(() =>
            new InstanceOrderedEventDto
            {
                DerivedInstanceId = src.DerivedInstanceId,
                SellerCompanyId = src.SellerCompanyId,
                BuyerCompanyId = src.BuyerCompanyId,
                DerivedInstanceItemId = src.DerivedInstanceItemId,
                Quantity = src.Quantity,
                UnitPrice = src.UnitPrice,
                TotalDiscountPercent = src.TotalDiscountPercent,
                ExpectedDeliveryDate = src.ExpectedDeliveryDate,

                InstanceId = src.InstanceId,
                Instance = Map(src.Instance, Map),

                CancelledEvent = src.CancelledEvent,
                SentEvent = src.SentEvent,
                DeliveredEvent = src.DeliveredEvent
            }, src);
}
