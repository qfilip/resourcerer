using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class DeleteUnitOfMeasure
{
    public class Handler : IAppHandler<Guid, UnitOfMeasureDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<UnitOfMeasureDto>> Handle(Guid request)
        {
            var entity = await _dbContext.UnitsOfMeasure
                .AsNoTracking()
                .Select(Utilities.Query.EntityBases.Expand<UnitOfMeasure>(x => new UnitOfMeasure
                {
                    Items = x.Items
                        .Select(i => new Item { Id = i.Id })
                        .ToList()
                }))
                .FirstOrDefaultAsync(x => x.Id == request);

            if (entity == null)
                return HandlerResult<UnitOfMeasureDto>.NotFound();

            if(entity.Items.Count > 0)
                return HandlerResult<UnitOfMeasureDto>.Rejected("Some items still exist associated with this unit of measure");

            _dbContext.MarkAsDeleted(entity);
            
            await _dbContext.SaveChangesAsync();

            return HandlerResult<UnitOfMeasureDto>.Ok(_mapper.Map<UnitOfMeasureDto>(entity));
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}

