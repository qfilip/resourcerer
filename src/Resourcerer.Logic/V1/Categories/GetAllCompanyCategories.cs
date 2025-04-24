using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Logic.Handlers;
using Resourcerer.Logic.Models;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;
using Resourcerer.Dtos.Records;
using MapsterMapper;

namespace Resourcerer.Logic.V1;

public static class GetAllCompanyCategories
{
    public class Handler : IAppHandler<Guid, CategoryDto[]>
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly Validator _validator;

        public Handler(AppDbContext appDbContext, IMapper mapper, Validator validator)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<HandlerResult<CategoryDto[]>> Handle(Guid companyId)
        {
            var entities = await _appDbContext.Categories
                .Where(x => x.CompanyId == companyId)
                .AsNoTracking()
                .ToArrayAsync();

            var result = entities
                .Select(_mapper.Map<CategoryDto>)
                .ToArray();

            return HandlerResult<CategoryDto[]>.Ok(result);
        }

        public ValidationResult Validate(Guid request) => _validator.Validate(request);
    }
    public class Validator : AbstractValidator<Guid>
    {
        public Validator()
        {
            RuleFor(x => x)
                .NotEmpty().WithMessage("Company Id cannot be empty");
        }
    }
}
