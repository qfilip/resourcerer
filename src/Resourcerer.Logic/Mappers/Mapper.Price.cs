using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static Price Map(PriceDto src) =>
        Map(() =>
            new Price
            {
                UnitValue = src.UnitValue,

                ItemId = src.ItemId,
                Item = Map(src.Item, Map)
            }, src);

    public static PriceDto Map(Price src) =>
        Map(() =>
            new PriceDto
            {
                UnitValue = src.UnitValue,

                ItemId = src.ItemId,
                Item = Map(src.Item, Map)
            }, src);
}
