using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.V1;
using Resourcerer.Identity.Abstractions;
using Resourcerer.Identity.Models;

namespace Resourcerer.Logic.V1;

public class UpdateCategory
{
    public class Handler : IAppHandler<V1UpdateCategory, CategoryDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;
        private readonly IAppIdentityService<AppIdentity> _identity;
        private readonly IMapper _mapper;

        public Handler(
            AppDbContext dbContext,
            Validator validator,
            IAppIdentityService<AppIdentity> identity,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _validator = validator;
            _identity = identity;
            _mapper = mapper;
        }

        public async Task<HandlerResult<CategoryDto>> Handle(V1UpdateCategory request)
        {
            var category = await _dbContext.Categories
                .FirstOrDefaultAsync(x =>
                    x.Id == request.CategoryId &&
                    x.CompanyId == _identity.Get().CompanyId
                );

            if (category == null)
                return HandlerResult<CategoryDto>.NotFound();

            if (request.NewParentCategoryId.HasValue && request.NewParentCategoryId != Guid.Empty)
            {
                var parentCategoryInfo = await _dbContext.Categories
                    .Select(x => new { x.Id, x.CompanyId })
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.NewParentCategoryId.Value &&
                        x.CompanyId == _identity.Get().CompanyId
                    );

                if (parentCategoryInfo == null)
                {
                    return HandlerResult<CategoryDto>.Rejected("Parent category not found");
                }

                category.ParentCategoryId = parentCategoryInfo.Id;
            }

            if (category.Name == request.NewName)
                return HandlerResult<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));

            category.Name = request.NewName;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<CategoryDto>.Ok(_mapper.Map<CategoryDto>(category));
        }

        public ValidationResult Validate(V1UpdateCategory request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1UpdateCategory>
    {
        public Validator()
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category id cannot be empty");

            RuleFor(x => x.NewName)
                .NotEmpty().WithMessage("Category name cannot be empty")
                .Length(min: 3, max: 50).WithMessage("Category name must be between 3 and 50 characters long");

            RuleFor(x => x)
                .Must(x => x.CategoryId != x.NewParentCategoryId)
                .WithMessage("Parent category ID cannot be the same as category ID that needs to be updated.");
        }
    }
}
