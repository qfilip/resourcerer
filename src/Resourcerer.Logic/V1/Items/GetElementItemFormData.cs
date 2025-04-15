using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class GetElementItemFormData
{
    public class Handler : IAppHandler<Guid, V1ElementItemFormData>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<V1ElementItemFormData>> Handle(Guid request)
        {
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

            return HandlerResult<V1ElementItemFormData>.Ok(new V1ElementItemFormData
            {
                Categories = categories.Select(_mapper.Map<CategoryDto>).ToArray(),
                UnitsOfMeasure = unitsOfMeasure.Select(_mapper.Map<UnitOfMeasureDto>).ToArray(),
            });
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
