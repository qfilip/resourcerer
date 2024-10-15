using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class DeleteUnitOfMeasure
{
    public class Handler : IAppHandler<Guid, UnitOfMeasureDto>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<UnitOfMeasureDto>> Handle(Guid request)
        {
            var entity = await _dbContext.UnitsOfMeasure
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request);

            if (entity == null)
                return HandlerResult<UnitOfMeasureDto>.NotFound();

            if(entity.Items.Count > 0)
                return HandlerResult<UnitOfMeasureDto>.Rejected("Some items still exist associated with this unit of measure");

            entity.EntityStatus = eEntityStatus.Deleted;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<UnitOfMeasureDto>.Ok(Mapper.Map(entity));
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}

