using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class GetCompanyItems
{
    public class Handler : IAppHandler<Guid, ItemDto[]>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<ItemDto[]>> Handle(Guid request)
        {
            var entities = await _dbContext.Items
                .Where(x => x.Category!.CompanyId == request)
                .Include(x => x.Prices)
                .ToArrayAsync();

            var result = entities
                .Select(Mapper.Map)
                .ToArray();

            return HandlerResult<ItemDto[]>.Ok(result);
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
