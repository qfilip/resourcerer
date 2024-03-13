using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos;
using Resourcerer.Logic.V1_0.Functions;
using System;

namespace Resourcerer.Logic.V1_0.Commands.Items;
public static class CreateItemProductionOrder
{
    public class Handler : IAppHandler<CreateItemProductionOrderRequestDto, Unit>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(CreateItemProductionOrderRequestDto request)
        {
            var item = _dbContext.Items
                .FirstOrDefault(x => x.Id == request.ItemId);

            if (item == null)
            {
                return HandlerResult<Unit>.Rejected("Item not found");
            }

            if(request.InstanceToUseIds.Length > 0)
            {
                var excerpts = await _dbContext.Excerpts
                .Where(x => x.CompositeId == request.ItemId)
                    .Include(x => x.Element)
                        .ThenInclude(x => x!.Instances)
                .ToArrayAsync();

                var instances = excerpts
                    .SelectMany(x => x.Element!.Instances)
                    .ToArray();

                var usableInstances = RequestedInstancesExist(excerpts, instances, request.InstanceToUseIds);
                if (usableInstances.Length == 0)
                {
                    return HandlerResult<Unit>.Rejected("Not all requested instances found");
                }

                PopulateRequiredQuantity(usableInstances, excerpts);
                // TODO

                // add ReservedEvents to Instance as json property
                // update ReservedEvents for every usable instance

                // add UsedEvents to Instance.ReservedEvent
            }

            var entity = new ItemProductionOrder
            {
                ItemId = item.Id,
                Quantity = request.Quantity
            };

            _dbContext.ItemProductionOrders.Add(entity);
            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        private static RequestedInstance[] RequestedInstancesExist(Excerpt[] excerpts, Instance[] instances, Guid[] requestedInstanceIds)
        {
            if(!instances.All(x => requestedInstanceIds.Contains(x.Id)))
            {
                return Array.Empty<RequestedInstance>();
            }

            var usableInstances = instances
                .Select(x => new RequestedInstance
                {
                    Id = x.Id,
                    ItemId = x.ItemId,
                    AvailableQuantity = Instances.GetAvailableUnitsInStock(x)
                })
                .ToArray();

            return usableInstances.All(x => requestedInstanceIds.Contains(x.Id)) ?
                usableInstances : Array.Empty<RequestedInstance>();
        }

        private static void PopulateRequiredQuantity(RequestedInstance[] usableInstances, Excerpt[] excerpts)
        {
            foreach (var ui in usableInstances)
            {
                var usableInstance = usableInstances.First(x => x.Id == ui.Id);
                var excerpt = excerpts.First(x => x.ElementId == usableInstance.ItemId);

                usableInstance.RequiredQuantity = excerpt.Quantity;
            }
        }

        public ValidationResult Validate(CreateItemProductionOrderRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
    
    private class RequestedInstance
    {
        public Guid Id { get; set; }
        public Guid? ItemId { get; set; }
        public double AvailableQuantity { get; set; }
        public double RequiredQuantity { get; set; }
    }
}

