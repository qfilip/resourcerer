using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.Entity;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;
public class GetItemInstances
{
    public class Handler : IAppHandler<Guid, InstanceDto[]>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<InstanceDto[]>> Handle(Guid request)
        {
            var result = await _dbContext.Instances
                .Where(i => i.ItemId == request)
                .AsNoTracking()
                .ToArrayAsync();

            var mapped = _mapper.Map<InstanceDto[]>(result);

            return HandlerResult<InstanceDto[]>.Ok(_mapper.Map<InstanceDto[]>(result));
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
