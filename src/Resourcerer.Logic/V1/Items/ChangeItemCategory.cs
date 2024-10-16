using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1;

public class ChangeItemCategory
{
    public class Handler : IAppHandler<V1ChangeItemCategory, ItemDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validatior;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, Validator validatior, IMapper mapper)
        {
            _dbContext = dbContext;
            _validatior = validatior;
            _mapper = mapper;
        }
        public async Task<HandlerResult<ItemDto>> Handle(V1ChangeItemCategory request)
        {
            var item = await _dbContext.Items
                .Select(x => new Item
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                })
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (item == null)
                return HandlerResult<ItemDto>.NotFound("Item not found");

            _dbContext.Attach(item);

            var newItemCategory = await _dbContext.Categories
                .FirstOrDefaultAsync(x => x.Id == request.NewCategoryId);

            if (newItemCategory == null)
                return HandlerResult<ItemDto>.Rejected("Category not found");

            item.CategoryId = newItemCategory.Id;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<ItemDto>.Ok(_mapper.Map<ItemDto>(item));
        }

        public ValidationResult Validate(V1ChangeItemCategory request) => _validatior.Validate(request);
    }

    public class Validator : AbstractValidator<V1ChangeItemCategory>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("Item id cannot be default value");

            RuleFor(x => x.NewCategoryId)
                .NotEmpty().WithMessage("New category id cannot be default value");
        }
    }
}
