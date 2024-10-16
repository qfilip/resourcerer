using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;

public static class GetCompanyItems
{
    public class Handler : IAppHandler<Guid, ItemDto[]>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<ItemDto[]>> Handle(Guid request)
        {
            var entities = await _dbContext.Items
                .Where(x => x.Category!.CompanyId == request)
                .Select(Utilities.Query.Items.Expand(x => new Item
                {
                    Prices = x.Prices
                }))
                .ToArrayAsync();

            var result = entities
                .Select(_mapper.Map<ItemDto>)
                .ToArray();

            return HandlerResult<ItemDto[]>.Ok(result);
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
