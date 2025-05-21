using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Logic.Utilities.Query;

namespace Resourcerer.Logic.V1;

public static class GetAllCompanies
{
    public class Handler : IAppHandler<Unit, CompanyDto[]>
    {
        private readonly AppDbContext _dbContext;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<CompanyDto[]>> Handle(Unit request)
        {
            var result = await _dbContext.Companies
                .Select(Companies.DefaultDtoProjection)
                .ToArrayAsync();

            return HandlerResult<CompanyDto[]>.Ok(result);
        }

        public ValidationResult Validate(Unit _) => new ValidationResult();
    }
}
