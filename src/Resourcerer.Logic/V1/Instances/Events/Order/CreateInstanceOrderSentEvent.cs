﻿using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Entities.JsonEntities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Instances.Events.Order;

public static class CreateInstanceOrderSentEvent
{
    public class Handler : IAppEventHandler<V1InstanceOrderSendCommand, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<HandlerResult<Unit>> Handle(V1InstanceOrderSendCommand request)
        {
            var orderEvent = await _appDbContext.InstanceOrderedEvents
                .FirstOrDefaultAsync(x =>
                    x.Id == request.OrderEventId &&
                    x.InstanceId == request.InstanceId);

            if (orderEvent == null)
            {
                return HandlerResult<Unit>.Rejected("Order not found");
            }

            if (orderEvent.CancelledEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Order was cancelled");
            }

            if (orderEvent.DeliveredEvent != null)
            {
                return HandlerResult<Unit>.Rejected("Order was delivered");
            }

            if (orderEvent.SentEvent != null)
            {
                return HandlerResult<Unit>.Ok(Unit.New);
            }

            orderEvent.SentEvent = AppDbJsonField
                .CreateKeyless(() => new InstanceOrderSentEvent());

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }
    public class Validator : AbstractValidator<V1InstanceOrderSendCommand>
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
