using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Fake;

namespace Resourcerer.Logic.Fake;

public static class FakeCommandEventHandler
{
    public class Handler : IAppEventHandler<FakeCommandDto, Unit>
    {
        private readonly AppDbContext _appDbContext;
        public Handler(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<HandlerResult<Unit>> Handle(FakeCommandDto request)
        {
            Console.WriteLine($"Command number: {request.Number}");
            
            var excerpts = await _appDbContext.Excerpts
                .ToArrayAsync();
            
            Console.WriteLine($"Excerpt count: {excerpts.Length}");

            return HandlerResult<Unit>.Ok(Unit.New);
        }
    }

    public class Validator : AbstractValidator<FakeCommandDto>
    {
        public Validator()
        {
            RuleFor(x => x.Number)
                .GreaterThan(0)
                .WithMessage("Number must be above 0.");
        }
    }
}
