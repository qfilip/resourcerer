using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entities;
using Resourcerer.Dtos.Records;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;
using Resourcerer.Logic.Utilities.Map;

namespace Resourcerer.Logic.V1;

public static class GetCompanyOverview
{
    public class Handler : IAppHandler<Guid, V1CompanyOverview>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<V1CompanyOverview>> Handle(Guid request)
        {
            var company = await _dbContext.Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request);
            
            if(company == null)
            {
                return HandlerResult<V1CompanyOverview>.NotFound();
            }

            var employees = await _dbContext.AppUsers
                .Where(x => x.CompanyId == request)
                .Select(x => new AppUserDto
                {
                    Name = x.Name,
                    IsAdmin = x.IsAdmin,
                    AuditRecord = AuditDto.Map(x.AuditRecord)
                })
                .AsNoTracking()
                .ToArrayAsync();


            var allCategories = await _dbContext.Categories
                .Where(x => x.CompanyId == request)
                .AsNoTracking()
                .ToArrayAsync();

            var categoryIds = allCategories.Select(x => x.Id).ToArray();

            var items = await _dbContext.Items
                .Where(x => categoryIds.Contains(x.CategoryId))
                .AsNoTracking()
                .ToArrayAsync();

            var categories = Categories.MapTree(allCategories);

            var categoryDtos = categories
                .Select(_mapper.Map<CategoryDto>)
                .ToArray();
            
            foreach (var category in categoryDtos)
            {
                category.Items = items
                    .Where(x => x.CategoryId == category.Id)
                    .Select(_mapper.Map<ItemDto>)
                    .ToArray();
            }

            var result = new V1CompanyOverview
            {
                Name = company.Name,
                Categories = categoryDtos,
                Employees = employees
            };

            return HandlerResult<V1CompanyOverview>.Ok(result);
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
