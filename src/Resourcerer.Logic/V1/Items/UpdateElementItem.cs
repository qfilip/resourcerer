using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Resourcerer.Logic.V1;

public class UpdateElementItem
{
    public class Handler : IAppHandler<V1UpdateElementItem, ItemDto>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, IMapper mapper, Validator validator)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HandlerResult<ItemDto>> Handle(V1UpdateElementItem request)
        {
            var item = await _appDbContext.Items
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if(item == null)
                return HandlerResult<ItemDto>.NotFound("Item not found");

            var category = await _appDbContext.Categories
                .Select(x => new
                {
                    x.Id
                })
                .FirstOrDefaultAsync(x => x.Id == request.CategoryId);

            if (category == null)
            {
                var error = "Category not found";
                return HandlerResult<ItemDto>.Rejected(error);
            }

            var uom = await _appDbContext.UnitsOfMeasure
                .FirstOrDefaultAsync(x => x.Id == request.UnitOfMeasureId);

            if (uom == null)
            {
                var error = "Requested unit of measure doesn't exist";
                return HandlerResult<ItemDto>.Rejected(error);
            }

            item.Name = request.Name;
            item.CategoryId = request.CategoryId;
            item.UnitOfMeasureId = request.UnitOfMeasureId;
            item.ProductionPrice = request.ProductionPrice;
            item.ProductionTimeSeconds = request.ProductionTimeSeconds;
            item.ExpirationTimeSeconds = request.ExpirationTimeSeconds;

            var price = new Price
            {
                Item = item,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<ItemDto>.Ok(_mapper.Map<ItemDto>(item));
        }

        public ValidationResult Validate(V1UpdateElementItem request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1UpdateElementItem>
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
