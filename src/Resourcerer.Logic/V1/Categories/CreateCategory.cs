using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.V1;

namespace Resourcerer.Logic.V1;

public class CreateCategory
{
    public class Handler : IAppHandler<V1CreateCategory, Unit>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<Unit>> Handle(V1CreateCategory request)
        {
            var existing = await _appDbContext.Categories
                .Where(x => x.CompanyId == request.CompanyId)
                .AsNoTracking()
                .ToArrayAsync();

            if (existing.Any(x => x.Name == request.Name && x.ParentCategoryId == request.ParentCategoryId))
            {
                var error = $"Category with name {request.Name} already exists";
                return HandlerResult<Unit>.Rejected(error);
            }

            if (request.ParentCategoryId != null)
            {
                if (!existing.Any(x => x.Id == request.ParentCategoryId))
                {
                    return HandlerResult<Unit>.Rejected("Parent category not found");
                }
            }

            var entity = new Category
            {
                Name = request.Name,
                CompanyId = request.CompanyId,
                ParentCategoryId = request.ParentCategoryId
            };

            _appDbContext.Categories.Add(entity);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }

        public ValidationResult Validate(V1CreateCategory request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1CreateCategory>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Category name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Category name must be between 3 and 50 characters long");

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company Id name cannot be empty");
        }
    }
}
