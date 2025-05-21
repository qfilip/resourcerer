using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using System.Linq.Expressions;

namespace Resourcerer.Logic.Utilities.Query;

public class Companies
{
    public static Expression<Func<Company, CompanyDto>> DefaultDtoProjection =
        EntityBases.Expand<Company, CompanyDto>(x => new CompanyDto
        {
            Name = x.Name
        });
}
