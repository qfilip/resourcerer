using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class GetItemShoppingList
{
    public class Handler : IAppHandler<Guid, V1ItemShoppingDetails[]>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<V1ItemShoppingDetails[]>> Handle(Guid request)
        {
            var itemsShoppingDetails = await _dbContext.Items
                .Where(x => x.Category!.CompanyId != request)
                .Select(x => new V1ItemShoppingDetails
                {
                    Id = x.Id,
                    Name = x.Name,
                    CategoryName = x.Category!.Name,
                    CompanyId = x.Category!.CompanyId,
                    CompanyName = x.Category!.Company!.Name
                })
                .ToArrayAsync();

            return HandlerResult<V1ItemShoppingDetails[]>.Ok(itemsShoppingDetails);
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
