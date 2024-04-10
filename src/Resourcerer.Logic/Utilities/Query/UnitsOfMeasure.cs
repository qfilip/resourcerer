using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

internal class UnitsOfMeasure
{
    public static Expression<Func<UnitOfMeasure, UnitOfMeasureDto>> DefaultDtoProjection =
        EntityBases.Expand<UnitOfMeasure, UnitOfMeasureDto>(x => new UnitOfMeasureDto
        {
            Name = x.Name,
            Symbol = x.Symbol,

            CompanyId = x.CompanyId
        });
}
