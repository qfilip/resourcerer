﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Exceptions;

using QU = Resourcerer.DataAccess.Utilities.Query.Instances;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class CancelItemProductionOrder
{
    public class Handler : IAppEventHandler<V1CancelItemProductionOrderRequest, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CancelItemProductionOrderRequest request)
        {
            var orderEvent = await _dbContext.ItemProductionOrders
                .FirstOrDefaultAsync(x => x.Id == request.ProductionOrderEventId);
            
            if(orderEvent == null)
            {
                return HandlerResult<Unit>.NotFound("Order event not found");
            }
            
            if(orderEvent.InstancesUsedIds.Length == 0)
            {
                var message = $"{nameof(orderEvent.InstancesUsedIds)} is empty for {nameof(ItemProductionOrder)} {orderEvent.Id}";
                throw new DataCorruptionException(message);
            }

            if (orderEvent.StartedEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Production already started");
            }

            var instances = await _dbContext.Instances
                .Where(x => orderEvent.InstancesUsedIds.Contains(x.Id))
                .Select(QU.Expand(x => new Instance
                {
                    ReservedEventsJson = x.ReservedEventsJson
                }))
                .AsNoTracking()
                .ToArrayAsync();

            if (orderEvent.InstancesUsedIds.Length != instances.Length)
            {
                var message = $"Not all used instances found for cancelling {nameof(ItemProductionOrder)} {orderEvent.Id}";
                throw new DataCorruptionException(message);
            }

            orderEvent.CanceledEvent = AppDbJsonField.Create(() =>
                new ItemProductionOrderCancelledEvent
                {
                    Reason = request.Reason
                });

            foreach(var i in instances)
            {
                var reservationEvent = i.ReservedEvents
                    .First(x =>
                        x.ProductionOrderId == orderEvent.Id &&
                        x.CancelledEvent == null &&
                        x.UsedEvent == null);

                reservationEvent.CancelledEvent = AppDbJsonField.Create(() =>
                    new InstanceReserveCancelledEvent
                    {
                        Reason = request.Reason
                    });

                _dbContext.Update(i);
            }

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public static ValidationResult Validate(V1CancelItemProductionOrderRequest request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<V1CancelItemProductionOrderRequest>
        {
            public Validator()
            {
                RuleFor(x => x.ProductionOrderEventId)
                    .NotEmpty().WithMessage("Order event id cannot be empty");
            }
        }
    }
}
