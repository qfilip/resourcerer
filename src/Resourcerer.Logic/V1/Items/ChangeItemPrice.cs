﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Items;

public class ChangeItemPrice
{
    public class Handler : IAppHandler<V1ChangePrice, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;
        private readonly ILogger<Handler> _logger;

        public Handler(AppDbContext appDbContext, Validator validator, ILogger<Handler> logger)
        {
            _appDbContext = appDbContext;
            _validator = validator;
            _logger = logger;
        }

        public async Task<HandlerResult<Unit>> Handle(V1ChangePrice request)
        {
            var element = await _appDbContext.Items
                .Where(x => x.Id == request.ItemId)
                .Include(x => x.Prices)
                .FirstOrDefaultAsync();

            if (element == null)
            {
                return HandlerResult<Unit>.NotFound($"Item with id {request.ItemId} doesn't exist");
            }

            if (element.Prices.Count(x => x.EntityStatus == eEntityStatus.Active) > 1)
            {
                _logger.LogWarning("Item with id {id} had multiple active prices", element.Id);
            }

            foreach (var p in element.Prices)
                p.EntityStatus = eEntityStatus.Deleted;

            var price = new Price
            {
                ItemId = request.ItemId,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(V1ChangePrice request) => _validator.Validate(request);

    }
    public class Validator : AbstractValidator<V1ChangePrice>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("Item id cannot be default value");

            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Item price must be greater than 0");
        }
    }
}
