using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic.V1;

public static class GetAllCompanyCategories
{
    public class Handler : IAppHandler<Guid, List<CategoryDto>>
    {
        private readonly AppDbContext _appDbContext;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, Validator validator)
        {
            _appDbContext = appDbContext;
            _validator = validator;
        }

        public async Task<HandlerResult<List<CategoryDto>>> Handle(Guid companyId)
        {
            var entities = await _appDbContext.Categories
                .Where(x => x.CompanyId == companyId)
                .AsNoTracking()
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
                ChildCategories = children.Select(x => MapDto(x, all)).ToArray(),
                AuditRecord = current.AuditRecord
            };
        }

        public ValidationResult Validate(Guid request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<Guid>
    {
        public Validator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Company Id cannot be empty");
        }
    }
}
