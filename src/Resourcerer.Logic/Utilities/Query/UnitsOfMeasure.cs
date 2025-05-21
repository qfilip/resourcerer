using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

internal class UnitsOfMeasure
{
    public static Expression<Func<UnitOfMeasure, UnitOfMeasure>> DefaultProjection =
        EntityBases.Expand<UnitOfMeasure>(x => new UnitOfMeasure
        {
            Name = x.Name,
            Symbol = x.Symbol,

            CompanyId = x.CompanyId
        });

    public static Expression<Func<UnitOfMeasure, UnitOfMeasureDto>> DefaultDtoProjection =
        EntityBases.Expand<UnitOfMeasure, UnitOfMeasureDto>(x => new UnitOfMeasureDto
        {
            Name = x.Name,
            Symbol = x.Symbol,

            CompanyId = x.CompanyId
        });

    public static Expression<Func<UnitOfMeasure, UnitOfMeasure>> Expand(Expression<Func<UnitOfMeasure, UnitOfMeasure>> projectionLayer)
    {
        return ExpressionUtils.Combine(DefaultProjection, projectionLayer);
    }
}
