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

public static class StartItemProductionOrder
{
    public class Handler : IAppEventHandler<V1StartItemProductionOrderCommand, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1StartItemProductionOrderCommand request)
        {
            var order = await _dbContext.ItemProductionOrders
                .Select(x => new ItemProductionOrder
                {
                    Id = x.Id,
                    InstancesUsedIds = x.InstancesUsedIds,
                    StartedEvent = x.StartedEvent,
                    CancelledEvent = x.CancelledEvent,
                    FinishedEvent = x.FinishedEvent,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderId);

            if (order == null)
            {
                return HandlerResult<Unit>.NotFound($"Production order {request.ProductionOrderId} not found");
            }

            if (order.CancelledEvent != null)
            {
                return HandlerResult<Unit>.Rejected($"Production order {request.ProductionOrderId} was cancelled and cannot be started");
            }

            if (order.FinishedEvent != null)
            {
                return HandlerResult<Unit>.Rejected($"Production order {request.ProductionOrderId} was finished and cannot be started");
            }

            if (order.StartedEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            var reservedEvents = await _dbContext.InstanceReservedEvents
                .Where(x => order.InstancesUsedIds.Contains(x.InstanceId))
                .ToArrayAsync();

            var instanceIds = reservedEvents
                .Select(x => x.InstanceId)
                .Distinct()
                .ToArray();

            if (order.InstancesUsedIds.Length != instanceIds.Length)
            {
                var message = $"Not all used instances found for consumption {nameof(ItemProductionOrder)} {order.Id}";
                throw new DataCorruptionException(message);
            }

            _dbContext.ItemProductionOrders.Attach(order);

            order.StartedEvent = AppDbJsonField.CreateKeyless(() => new ItemProductionStartedEvent());

            foreach (var ev in reservedEvents)
            {
                ev.UsedEvent = AppDbJsonField.CreateKeyless(() => new InstanceReserveUsedEvent());
            }

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }
    public class Validator : AbstractValidator<V1StartItemProductionOrderCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ProductionOrderId)
                .NotEmpty().WithMessage("Production order id cannot be empty");
        }
    }
}
