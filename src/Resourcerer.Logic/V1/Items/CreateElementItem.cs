﻿using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1.Items;

public static class CreateElementItem
{
    public class Handler : IAppHandler<V1CreateElementItem, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateElementItem request)
        {
            var category = await _appDbContext.Categories
                .Select(x => new
                {
                    x.Id
                })
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Category not found";
                return HandlerResult<Unit>.Rejected(error);
            }

            var existing = await _appDbContext.Items
                .FirstOrDefaultAsync(x =>
                    x.Name == request.Name &&
                    x.CategoryId == request.CategoryId);

            if (existing != null)
            {
                var error = "Element with the same name and category already exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
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

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(V1CreateElementItem request) => _validator.Validate(request);

    }
    public class Validator : AbstractValidator<V1CreateElementItem>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Element name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Element name must be between 3 and 50 characters long");

            RuleFor(x => x.ProductionTimeSeconds)
                .GreaterThanOrEqualTo(0).WithMessage("ProductionTimeSeconds cannot be negative");

            RuleFor(x => x.ExpirationTimeSeconds)
                .Must(x =>
                {
                    if (x == null) return true;
                    else return x < 0;
                }).WithMessage("ExpirationTimeSeconds cannot be negative");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Element's category cannot be empty");

            RuleFor(x => x.UnitOfMeasureId)
                .NotEmpty().WithMessage("Element's unit of measure cannot be empty");

            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Element's price must be greater than 0");
        }
    }
}
