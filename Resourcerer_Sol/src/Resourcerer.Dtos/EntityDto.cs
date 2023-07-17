using Resourcerer.DataAccess.Enums;

namespace Resourcerer.Dtos;

public abstract class EntityDto<T> : BaseDto<T>
{
    public Guid Id { get; set; }
    public eEntityStatus EntityStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
