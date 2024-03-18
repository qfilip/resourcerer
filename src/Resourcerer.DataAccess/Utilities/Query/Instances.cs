using Resourcerer.DataAccess.Entities;
using System.Linq.Expressions;

namespace Resourcerer.DataAccess.Utilities.Query;

public class Instances
{
    public static Expression<Func<Instance, Instance>> DefaultProjection =
        EntityBases.Expand<Instance>(x => new Instance
        {
            Quantity = x.Quantity,
            ExpiryDate = x.ExpiryDate,

            ItemId = x.ItemId,
            OwnerCompanyId = x.OwnerCompanyId,
            SourceInstanceId = x.SourceInstanceId,
        });

    public static Expression<Func<Instance, Instance>> Expand(Expression<Func<Instance, Instance>> projectionLayer)
    {
        return ExpressionUtils.Combine(DefaultProjection, projectionLayer);
    }
}
