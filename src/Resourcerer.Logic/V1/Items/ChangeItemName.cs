using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class ChangeItemName
{
    public class Handler : IAppHandler<V1ChangeItemName, ItemDto>
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

        public async Task<HandlerResult<ItemDto>> Handle(V1ChangeItemName request)
        {
            var item = await _dbContext.Items
                .FirstOrDefaultAsync(x => x.Id == request.ItemId);

            if (item == null)
                return HandlerResult<ItemDto>.NotFound("Item not found");

            item.Name = request.NewName;
            
            await _dbContext.SaveChangesAsync();

            return HandlerResult<ItemDto>.Ok(_mapper.Map<ItemDto>(item));
        }

        public ValidationResult Validate(V1ChangeItemName request) => _validatior.Validate(request);
    }

    public class Validator : AbstractValidator<V1ChangeItemName>
    {
        public Validator()
        {
            RuleFor(x => x.ItemId)
                .NotEmpty().WithMessage("Item id cannot be default value");

            RuleFor(x => x.NewName)
                .Must(Validation.Item.Name)
                .WithMessage(Validation.Item.NameError);
        }
    }
}
