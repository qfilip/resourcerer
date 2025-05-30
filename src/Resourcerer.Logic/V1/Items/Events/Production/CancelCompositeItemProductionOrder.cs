﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;

namespace Resourcerer.Logic.V1.Items.Events.Production;
public static class CancelCompositeItemProductionOrder
{
    public class Handler : IAppEventHandler<V1CancelCompositeItemProductionOrderCommand, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CancelCompositeItemProductionOrderCommand request)
        {
            var orderEvent = await _dbContext.ItemProductionOrders
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderId);

            if (orderEvent == null)
            {
                return HandlerResult<Unit>.NotFound("Order event not found");
            }

            if (orderEvent.InstancesUsedIds.Length == 0)
            {
                var message = $"{nameof(orderEvent.InstancesUsedIds)} is empty for {nameof(ItemProductionOrder)} {orderEvent.Id}";
                throw new DataCorruptionException(message);
            }

            if (orderEvent.StartedEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production already started");
            }

            if (orderEvent.FinishedEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production already finished");
            }

            var reservedEvents = await _dbContext.InstanceReservedEvents
                .Where(x => orderEvent.InstancesUsedIds.Contains(x.InstanceId))
                .ToArrayAsync();

            foreach (var ev in reservedEvents)
            {
                ev.CancelledEvent = AppDbJsonField.CreateKeyless(() =>
                    new InstanceReserveCancelledEvent() { Reason = request.Reason });
            }

            orderEvent.CancelledEvent = AppDbJsonField.CreateKeyless(() =>
                new ItemProductionOrderCancelledEvent() { Reason = request.Reason });

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }
    public class Validator : AbstractValidator<V1CancelCompositeItemProductionOrderCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ProductionOrderId)
                .NotEmpty().WithMessage("Order event id cannot be empty");
        }
    }
}
