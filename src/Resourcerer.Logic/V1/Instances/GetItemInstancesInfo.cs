using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.Dtos.V1;
using Resourcerer.Logic.Models;
using Resourcerer.Logic.Utilities;

namespace Resourcerer.Logic.V1;
public class GetItemInstancesInfo
{
    public class Handler : IAppHandler<Guid, V1InstanceInfo[]>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;

        public Handler(AppDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<HandlerResult<V1InstanceInfo[]>> Handle(Guid request)
        {
            var instances = await _dbContext.Instances
                .Where(i => i.ItemId == request)
                .Include(x => x.SourceInstance)
                .AsNoTracking()
                .ToArrayAsync();
            
            var result = instances
                .Select(x => Functions.Instances.GetInstanceInfo(x, DateTime.UtcNow))
                .ToArray();

            return HandlerResult<V1InstanceInfo[]>.Ok(_mapper.Map<V1InstanceInfo[]>(result));
        }

        public ValidationResult Validate(Guid _) => Validation.Empty;
    }
}
