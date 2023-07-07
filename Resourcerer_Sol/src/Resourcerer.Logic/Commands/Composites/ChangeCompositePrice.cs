﻿using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;

namespace Resourcerer.Logic.Commands.Composites;

public static class ChangeCompositePrice
{
    public class Handler : IAppHandler<ChangePriceDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(ChangePriceDto request)
        {
            var prices = await _appDbContext.Prices
                .Where(x => x.CompositeId == request.EntityId)
                .ToListAsync();

            if (!prices.Any())
            {
                return HandlerResult<Unit>.NotFound($"Price for entity with id {request.EntityId} not found");
            }

            if (prices.Count > 1)
            {
                // report error
            }

            prices.ForEach(x => x.EntityStatus = eEntityStatus.Deleted);

            var price = new Price
            {
                CompositeId = request.EntityId,
                UnitValue = request.UnitPrice
            };

            _appDbContext.Prices.Add(price);
            await _appDbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(new Unit());
        }
    }
}
