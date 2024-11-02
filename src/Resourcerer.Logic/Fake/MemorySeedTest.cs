using FluentValidation.Results;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Utilities.Faking;
using Resourcerer.Dtos;
using Resourcerer.Identity.Utils;
using SqlForgery;
using System.Text.Json;

namespace Resourcerer.Logic.Fake;

public static class MemorySeedTest
{
    public class Handler : IAppHandler<int, Unit>
    {
        private readonly AppDbContext _dbContext;
        private readonly Forger _forger;
        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _forger = new(dbContext, DF.FakingFunctions);
        }

        public async Task<HandlerResult<Unit>> Handle(int excerpts)
        {
            var allPermissions = Permissions.GetCompressed();
            var adminPermissoins = JsonSerializer.Serialize(allPermissions);

            var _ = Enumerable.Range(0, excerpts)
                .Select(_ => _forger.Fake<Excerpt>())
                .ToArray();

            await _dbContext.SaveChangesAsync();

            return HandlerResult<Unit>.Ok(Unit.New);
        }

        public ValidationResult Validate(int request) => new ValidationResult();
    }
}

