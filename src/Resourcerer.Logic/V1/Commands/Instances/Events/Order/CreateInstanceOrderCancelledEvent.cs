﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Commands;

public static class CreateInstanceOrderCancelledEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderCancelRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderCancelRequest request)
        {
            var instance = await _appDbContext.Instances
                .Select(x => new Instance
                {
                    Id = x.Id,
                    OrderedEvents = x.OrderedEvents
                        .Where(x => x.Id == request.OrderEventId)
                        .ToArray()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InstanceId);


            if (instance == null)
            {
                var error = $"Instance with id {request.InstanceId} not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            var orderEvent = instance.OrderedEvents
                .FirstOrDefault(x => x.Id == request.OrderEventId);

            if (orderEvent == null)
            {
                var error = "Order event not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.SentEvent != null)
            {
                var error = "Order sent and cannot be cancelled";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.DeliveredEvent != null)
            {
                var error = "Order already delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.CancelledEvent != null)
            {
                return HandlerResult<Unit>.Ok(new Unit());
            }

            var cancelEvent = AppDbJsonField.Create(() =>
            {
                return new InstanceOrderCancelledEvent
                {
                    Reason = request.Reason,
                    RefundedAmount = request.RefundedAmount
                };
            });
            
            _appDbContext.InstanceOrderedEvents.Attach(orderEvent);
            orderEvent.CancelledEvent = cancelEvent;
            
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public static ValidationResult Validate(V1InstanceOrderCancelRequest request) =>
            new Validator().Validate(request);

        private class Validator : AbstractValidator<V1InstanceOrderCancelRequest>
        {
            public Validator()
            {
                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("Instance id cannot be empty");

                RuleFor(x => x.OrderEventId)
                    .NotEmpty()
                    .WithMessage("Order event id cannot be empty");

                RuleFor(x => x.Reason)
                    .NotNull()
                    .NotEmpty()
                    .WithMessage("Reason cannot be empty");

                RuleFor(x => x.RefundedAmount)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Refunded Amount must be equal or greater than 0");
            }
        }
    }
}
