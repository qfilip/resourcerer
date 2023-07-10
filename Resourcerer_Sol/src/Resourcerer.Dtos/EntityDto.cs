using FluentValidation;

namespace Resourcerer.Dtos;

public abstract class EntityDto<T> : IBaseDto<T>
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public abstract AbstractValidator<T> GetValidator();
}
