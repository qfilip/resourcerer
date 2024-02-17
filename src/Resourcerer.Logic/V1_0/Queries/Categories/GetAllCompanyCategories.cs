using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Queries.V1_0;

public static class GetAllCompanyCategories
{
    public class Handler : IAppHandler<Guid, List<CategoryDto>>
    {
        private readonly AppDbContext _appDbContext;

        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<List<CategoryDto>>> Handle(Guid companyId)
        {
            var entities = await _appDbContext.Categories
                .Where(x => x.CompanyId == companyId)
                .ToArrayAsync();

            var result = entities
                .Where(x => x.ParentCategoryId == null)
                .Select(x => MapDto(x, entities))
                .ToList();

            return HandlerResult<List<CategoryDto>>.Ok(result);
        }

        private static CategoryDto MapDto(Category current, Category[] all)
        {
            var children = all
                .Where(x => x.ParentCategoryId == current.Id)
                .ToArray();

            return new CategoryDto
            {
                Id = current.Id,
                Name = current.Name,
                ParentCategoryId = current.ParentCategoryId,
                ChildCategories = children.Select(x => MapDto(x, all)).ToList(),
                CreatedAt = current.CreatedAt,
                ModifiedAt = current.ModifiedAt
            };
        }

        public ValidationResult Validate(Guid _) => new ValidationResult();

        private class Validator : AbstractValidator<Guid>
        {
            public Validator()
            {
                RuleFor(x => x)
                    .NotEmpty().WithMessage("Company Id cannot be empty");
            }
        }
    }
}
