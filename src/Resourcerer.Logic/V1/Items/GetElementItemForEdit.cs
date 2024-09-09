using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class GetElementItemForEdit
{
    public class Handler : IAppHandler<(Guid ItemId, Guid CompanyId), V1EditElementItemFormData>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<V1EditElementItemFormData>> Handle((Guid ItemId, Guid CompanyId) request)
        {
            var item = await _dbContext.Items
                .Select(Utilities.Query.Items.Expand(x => new Item
                {
                    Prices = x.Prices,
                    Category = x.Category,
                    UnitOfMeasure = x.UnitOfMeasure,
                }))
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if(item == null)
            {
                return HandlerResult<V1EditElementItemFormData>.NotFound();
            }

            var categories = await _dbContext.Categories
                .Where(x => x.CompanyId == request.CompanyId)
                .ToArrayAsync();

            var unitsOfMeasure = await _dbContext.UnitsOfMeasure
                .Where(x => x.CompanyId == request.CompanyId)
                .ToArrayAsync();

            return HandlerResult<V1EditElementItemFormData>.Ok(new V1EditElementItemFormData
            {
                Item = Mapper.Map(item),
                Categories = categories.Select(Mapper.Map).ToArray(),
                UnitsOfMeasure = unitsOfMeasure.Select(Mapper.Map).ToArray(),
            });
        }

        public ValidationResult Validate((Guid ItemId, Guid CompanyId) _) => Validation.Empty;
    }
}
