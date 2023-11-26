using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.V1_0;

public class CreateCategory
{
    public class Handler : IAppHandler<CategoryDto, Unit>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CategoryDto request)
        {
            var existing = await _appDbContext.Categories
                .Where(x =>
                    x.Name == request.Name ||
                    x.Id == request.ParentCategoryId)
                .ToListAsync();

            if(existing.Any(x => x.Name == request.Name))
            {
                var error = $"Category with name {request.Name} already exists";
                return HandlerResult<Unit>.Rejected(error);
            }

            if(request.ParentCategoryId != null && !existing.Any(x => x.Id == request.ParentCategoryId))
            {
                var error = $"Parent category with id {request.ParentCategoryId} doesn't exist";
                return HandlerResult<Unit>.Rejected(error);
            }

            var entity = new Category
            {
                Name = request.Name,
                ParentCategoryId = request.ParentCategoryId
            };

            _appDbContext.Categories.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(CategoryDto request)
        {
            var validator = new Validator();
            return validator.Validate(request);
        }

        private class Validator : AbstractValidator<CategoryDto>
        {
            public Validator()
            {
                RuleFor(x => x.Name)
                    .NotEmpty().WithMessage("Category name cannot be empty")
                    .Length(min: 3, max: 50).WithMessage("Category name must be between 3 and 50 characters long");
            }
        }
    }
}
