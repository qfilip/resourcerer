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

namespace Resourcerer.Logic.V1;

public class CreateCategory
{
    public class Handler : IAppHandler<V1CreateCategory, CategoryDto>
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

        public async Task<HandlerResult<CategoryDto>> Handle(V1CreateCategory request)
        {
            var existing = await _appDbContext.Categories
                .Where(x => x.CompanyId == request.CompanyId)
                .AsNoTracking()
                .ToArrayAsync();

            if (existing.Any(x => x.Name == request.Name && x.ParentCategoryId == request.ParentCategoryId))
            {
                var error = $"Category with name {request.Name} already exists";
                return HandlerResult<CategoryDto>.Rejected(error);
            }

            if (request.ParentCategoryId != null)
            {
                if (!existing.Any(x => x.Id == request.ParentCategoryId))
                {
                    return HandlerResult<CategoryDto>.Rejected("Parent category not found");
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

            var dto = _mapper.Map<CategoryDto>(entity);
            return HandlerResult<CategoryDto>.Ok(dto);
        }

        public ValidationResult Validate(V1CreateCategory request) => _validator.Validate(request);
    }

    public class Validator : AbstractValidator<V1CreateCategory>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .Must(Validation.Category.Name)
                .WithMessage(Validation.Category.NameError);

            RuleFor(x => x.CompanyId)
                .NotEmpty().WithMessage("Company Id name cannot be empty");
        }
    }
}
