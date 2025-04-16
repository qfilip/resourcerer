using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Enums;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public class GetItemType
{
    public class Handler : IAppHandler<Guid, eItemType>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<eItemType>> Handle(Guid request)
        {
            var entityInfo = await _dbContext.Items
                .Select(x => new
                {
                    x.Id,
                    IsComposite = x.Recipes.Count > 0,
                }).FirstOrDefaultAsync(x => x.Id == request);

            var ds = _dbContext.Recipes.ToArray();
            if (entityInfo == null)
                return HandlerResult<eItemType>.NotFound();

            var type = entityInfo.IsComposite ? eItemType.Composite : eItemType.Element;
            
            return  HandlerResult<eItemType>.Ok(type);
        }

        public ValidationResult Validate(Guid request) => Validation.Empty;
    }
}
