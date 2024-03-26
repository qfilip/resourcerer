using FluentValidation;
using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Functions.V1;

namespace Resourcerer.Logic.V1.Commands;

public static class RemoveCategory
{
    public class Handler : IAppHandler<CategoryDto, Guid>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext dbContext, Validator validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<Guid>> Handle(CategoryDto request)
        {
            return await EntityAction
                .Remove<Category>(_dbContext, request.Id, $"Category with id {request.Id} doesn't exist");
        }

        public ValidationResult Validate(CategoryDto request) => _validator.Validate(request);

    }
    public class Validator : AbstractValidator<CategoryDto>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Category Id cannot be empty");
        }
    }
}
