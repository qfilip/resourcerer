using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic.V1;

public static class RemoveCategory
{
    public class Handler : IAppHandler<CategoryDto, CategoryDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext dbContext, Validator validator)
        {
            _dbContext = dbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<CategoryDto>> Handle(CategoryDto request)
        {
            var entity = await _dbContext.Categories
                .Include(c => c.ChildCategories)
                .FirstOrDefaultAsync(x => x.Id == request.Id);

            if (entity == null)
                return HandlerResult<CategoryDto>.NotFound();

            entity.EntityStatus = eEntityStatus.Deleted;
            
            foreach (var cc in entity.ChildCategories)
                cc.ParentCategoryId = null;

            await _dbContext.SaveChangesAsync();

            return HandlerResult<CategoryDto>.Ok(new CategoryDto
            {
                Id = entity.Id
            });
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
