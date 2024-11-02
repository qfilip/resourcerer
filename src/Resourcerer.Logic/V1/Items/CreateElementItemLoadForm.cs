using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class CreateElementItemLoadForm
{
    public class Handler : IAppHandler<Guid, V1CreateElementItemFormDataDto>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<V1CreateElementItemFormDataDto>> Handle(Guid request)
        {
            var categories = await _appDbContext.Categories
                .Where(x => x.CompanyId == request)
                .Select(x => new CategoryDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .AsNoTracking()
                .ToArrayAsync();

            var unitsOfMeasure = await _appDbContext.UnitsOfMeasure
                .Where(x => x.CompanyId == request)
                .Select(x => new UnitOfMeasureDto
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .AsNoTracking()
                .ToArrayAsync();

            return HandlerResult<V1CreateElementItemFormDataDto>.Ok(new()
            {
                CompanyId = request,
                Categories = categories,
                UnitsOfMeasure = unitsOfMeasure
            });
        }

        public ValidationResult Validate(Guid request) => Validation.Empty;
    }
}
