using FluentValidation;
using FluentValidation.Results;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Functions.V1;

namespace Resourcerer.Logic.V1.Commands;

public static class RemoveCategory
{
    public class Handler : IAppHandler<CategoryDto, Guid>
    {
        private readonly AppDbContext _dbContext;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Guid>> Handle(CategoryDto request)
        {
            return await EntityAction
                .Remove<Category>(_dbContext, request.Id, $"Category with id {request.Id} doesn't exist");
        }

        public ValidationResult Validate(CategoryDto request)
        {
            return new Validator().Validate(request);
        }

        private class Validator : AbstractValidator<CategoryDto>
        {
            public Validator()
            {
                RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Category Id cannot be empty");
            }
        }
    }
}
