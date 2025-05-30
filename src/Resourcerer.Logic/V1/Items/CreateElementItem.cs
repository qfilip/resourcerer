﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;
using Resourcerer.Dtos.Entities;
using MapsterMapper;

namespace Resourcerer.Logic.V1.Items;

public static class CreateElementItem
{
    public class Handler : IAppHandler<V1CreateElementItem, ItemDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<ItemDto>> Handle(V1CreateElementItem request)
        {
            var category = await _appDbContext.Categories
                .Select(x => new
                {
                    x.Id
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Category not found";
                return HandlerResult<ItemDto>.Rejected(error);
            }

            var existing = await _appDbContext.Items
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                    x.Name == request.Name &&
                    x.CategoryId == request.CategoryId);

            if (existing != null)
            {
                var error = "Element with the same name and category already exist";
                return HandlerResult<ItemDto>.Rejected(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<ItemDto>.Rejected(error);
            }

            var item = new Item
            {
                Id = Guid.NewGuid(),
                ProductionTimeSeconds = request.ProductionTimeSeconds,
                ExpirationTimeSeconds = request.ExpirationTimeSeconds,
                Name = request.Name,
                CategoryId = request.CategoryId,
                UnitOfMeasureId = request.UnitOfMeasureId
            };

            var price = new Price
            {
                Item = item,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Items.Add(item);
            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<ItemDto>.Ok(new ItemDto
            {
                Id = item.Id,
                ProductionTimeSeconds = item.ProductionTimeSeconds,
                ExpirationTimeSeconds = item.ExpirationTimeSeconds,
                Name = item.Name,
                CategoryId = item.CategoryId,
                UnitOfMeasureId = item.UnitOfMeasureId
            });
        }

        public ValidationResult Validate(V1CreateElementItem request) => _validator.Validate(request);

    }
    public class Validator : AbstractValidator<V1CreateElementItem>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .Must(Validation.Item.Name)
                .WithMessage(Validation.Item.NameError);

            RuleFor(x => x.ProductionTimeSeconds)
                .Must(Validation.Item.ProductionTimeSeconds)
                .WithMessage(Validation.Item.ProductionTimeSecondsError);

            RuleFor(x => x.ExpirationTimeSeconds)
                .Must(Validation.Item.ExpirationTimeSeconds)
                .WithMessage(Validation.Item.ExpirationTimeSecondsError);

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Element's category cannot be empty");

            RuleFor(x => x.UnitOfMeasureId)
                .NotEmpty().WithMessage("Element's unit of measure cannot be empty");

            RuleFor(x => x.UnitPrice)
                .Must(Validation.Item.Price)
                .WithMessage(Validation.Item.PriceError);
        }
    }
}
