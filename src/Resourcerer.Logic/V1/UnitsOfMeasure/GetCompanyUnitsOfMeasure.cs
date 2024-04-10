using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Utilities;
using Resourcerer.Logic.Utilities.Query;

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
                .Select(UnitsOfMeasure.DefaultDtoProjection)
                .AsNoTracking()
                .ToArrayAsync();

            return HandlerResult<UnitOfMeasureDto[]>.Ok(result);
        }

        public ValidationResult Validate(Guid request) => Validation.Empty;
    }
}
