using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class RemoveItem
{
    public class Handler : IAppHandler<Guid, ItemDto>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<ItemDto>> Handle(Guid request)
        {
            var item = await _dbContext.Items.Select(x => new Item
            {
                Id = x.Id,
                Name = x.Name,
                EntityStatus = x.EntityStatus,
                ProductionOrders = x.ProductionOrders
            }).FirstOrDefaultAsync(x => x.Id == request);

            if (item == null)
                return HandlerResult<ItemDto>.NotFound();

            var allOrdersResolved = item.ProductionOrders
                .All(x => x.CancelledEvent != null || x.FinishedEvent != null);

            if (!allOrdersResolved)
                return HandlerResult<ItemDto>.Rejected("All item orders must be resolved before removing it");


            _dbContext.MarkAsDeleted(item);
            await _dbContext.SaveChangesAsync();

            return HandlerResult<ItemDto>.Ok(new ItemDto { Id = item.Id, Name = item.Name });
        }

        public ValidationResult Validate(Guid request) => Validation.Empty;
    }
}
