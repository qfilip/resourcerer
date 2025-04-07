using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;

namespace Resourcerer.Logic.V1;

public static class GetExamples
{
    public class Handler : IAppHandler<Unit, V1ExampleDto[]>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<V1ExampleDto[]>> Handle(Unit request)
        {
            var entities = await _dbContext.Examples.ToArrayAsync();
            var result = entities.Select(_mapper.Map<V1ExampleDto>).ToArray();

            return HandlerResult<V1ExampleDto[]>.Ok(result);
        }

        public ValidationResult Validate(Unit _) => new ValidationResult();
    }
}
