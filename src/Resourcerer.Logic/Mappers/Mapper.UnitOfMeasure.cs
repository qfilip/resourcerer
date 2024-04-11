using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static UnitOfMeasure Map(UnitOfMeasureDto src) =>
        Map(() =>
            new UnitOfMeasure
            {
                Name = src.Name,
                Symbol = src.Symbol,

                CompanyId = src.CompanyId,
                Company = Map(src.Company, Map),
                
                Items = src.Items.Select(Map).ToArray()
            }, src);

    public static UnitOfMeasureDto Map(UnitOfMeasure src) =>
        Map(() =>
            new UnitOfMeasureDto
            {
                Name = src.Name,
                Symbol = src.Symbol,

                CompanyId = src.CompanyId,
                Company = Map(src.Company, Map),

                Items = src.Items.Select(Map).ToArray()
            }, src);
}
