using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class GetCompositeItemFormData
{
    public class Handler : IAppHandler<Guid, V1CompositeItemFormData>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<V1CompositeItemFormData>> Handle(Guid request)
        {
            var items = await _dbContext.Items
                .Where(x => x.Category!.CompanyId == request)
                .Select(x => new ItemDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .AsNoTracking()
                .ToArrayAsync();

            var categories = await _dbContext.Categories
                .Where(x => x.CompanyId == request)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .AsNoTracking()
                .ToArrayAsync();

            var unitsOfMeasure = await _dbContext.UnitsOfMeasure
                .Where(x => x.CompanyId == request)
                .Select(x => new UnitOfMeasureDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .AsNoTracking()
                .ToArrayAsync();

            return HandlerResult<V1CompositeItemFormData>.Ok(new()
            {
                Items = items,
                Categories = categories,
                UnitsOfMeasure = unitsOfMeasure
            });
        }

        public ValidationResult Validate(Guid request) => Validation.Empty;
    }
}
