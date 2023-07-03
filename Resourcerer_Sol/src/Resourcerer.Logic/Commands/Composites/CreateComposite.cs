﻿using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Composites;

namespace Resourcerer.Logic.Commands.Composites;

public static class CreateComposite
{
    public class Handler : IRequestHandler<CreateCompositeDto, Unit>
    {
        private readonly IAppDbContext _appDbContext;

        public Handler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateCompositeDto request)
        {
            var composite = new Composite
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Name = request.Name
            };

            var excerpts = request.Elements!
                .Select(x => new Excerpt
                {
                    CompositeId = composite.Id,
                    ElementId = x.ElementId,
                    Quantity = x.Quantity
                });

            var price = new Price
            {
                CompositeId = composite.Id,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Composites.Add(composite);
            _appDbContext.Excerpts.AddRange(excerpts);
            _appDbContext.Prices.Add(price);

            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
