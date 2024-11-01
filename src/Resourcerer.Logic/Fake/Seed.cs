using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Handlers;
using Resourcerer.Application.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos.Fake;
using Resourcerer.Identity.Utils;
using SqlForgery;
using System.Text.Json;

namespace Resourcerer.Logic.Fake;

public static class Seed
{
    public class Handler : IAppHandler<Unit, DataSeedDto>
    {
        private readonly AppDbContext _dbContext;
        private readonly Forger _forger;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _forger = new(dbContext, DF.FakingFunctions);
        }

        public async Task<HandlerResult<DataSeedDto>> Handle(Unit _)
        {
            var allPermissions = Permissions.GetCompressed();
            var adminPermissoins = JsonSerializer.Serialize(allPermissions);

            DF.FakeDatabase(_forger, adminPermissoins);

            await _dbContext.SaveChangesAsync();

            Console.WriteLine("DATABASE SEEDED");

            var result = new DataSeedDto
            {
                Companies = _dbContext.Companies.AsNoTracking().ToArray(),
                AppUsers = _dbContext.AppUsers.AsNoTracking().ToArray(),
                Categories = _dbContext.Categories.AsNoTracking().ToArray(),
                UnitsOfMeasure = _dbContext.UnitsOfMeasure.AsNoTracking().ToArray(),
                Excerpts = _dbContext.Excerpts.AsNoTracking().ToArray(),
                Prices = _dbContext.Prices.AsNoTracking().ToArray(),
                Items = _dbContext.Items.AsNoTracking().ToArray(),
                ItemProductionOrders = _dbContext.ItemProductionOrders.AsNoTracking().ToArray(),
                Instances = _dbContext.Instances.AsNoTracking().ToArray(),
                InstanceOrderedEvents = _dbContext.InstanceOrderedEvents.AsNoTracking().ToArray(),
                InstanceReservedEvents = _dbContext.InstanceReservedEvents.AsNoTracking().ToArray(),
                InstanceDiscardedEvents = _dbContext.InstanceDiscardedEvents.AsNoTracking().ToArray()
            };

            return HandlerResult<DataSeedDto>.Ok(result);
        }

        public ValidationResult Validate(Unit _) => new ValidationResult();
    }
}
