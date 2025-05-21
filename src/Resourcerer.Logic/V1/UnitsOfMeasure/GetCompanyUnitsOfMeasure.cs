using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class GetCompanyUnitsOfMeasure
{
    public class Handler : IAppHandler<Guid, UnitOfMeasureDto[]>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<UnitOfMeasureDto[]>> Handle(Guid request)
        {
            var result = await _dbContext.UnitsOfMeasure
                .Where(u => u.CompanyId == request)
                .Select(Utilities.Query.UnitsOfMeasure.DefaultDtoProjection)
                .AsNoTracking()
                .ToArrayAsync();

            return HandlerResult<UnitOfMeasureDto[]>.Ok(result);
        }

        public ValidationResult Validate(Guid request) => Validation.Empty;
    }
}
