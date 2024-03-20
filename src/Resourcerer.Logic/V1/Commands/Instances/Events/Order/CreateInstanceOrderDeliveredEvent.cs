﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Commands;

public static class CreateInstanceOrderDeliveredEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderDeliveredRequest, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderDeliveredRequest request)
        {
            var orderEvent = await _appDbContext.InstanceOrderedEvents
                .FirstOrDefaultAsync(x =>
                    x.Id == request.OrderEventId &&
                    x.InstanceId == request.InstanceId);

            if (orderEvent == null)
            {
                var error = "Order event not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            if(orderEvent.SentEvent == null)
            {
                var error = "Instance was not sent, so it cannot be delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.CancelledEvent != null)
            {
                var error = "Order was cancelled, so it cannot be delivered";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (orderEvent.DeliveredEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            orderEvent.DeliveredEvent = AppDbJsonField
                .Create(() => new InstanceOrderDeliveredEvent()); ;
            
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public static ValidationResult Validate(V1InstanceOrderDeliveredRequest request) =>
            new Validator().Validate(request);

        public class Validator : AbstractValidator<V1InstanceOrderDeliveredRequest>
        {
            public Validator()
            {
                RuleFor(x => x.InstanceId)
                    .NotEmpty()
                    .WithMessage("Instance id cannot be empty");

                RuleFor(x => x.OrderEventId)
                    .NotEmpty()
                    .WithMessage("Order event id cannot be empty");
            }
        }
    }
}
