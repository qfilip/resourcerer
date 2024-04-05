using Resourcerer.DataAccess.Enums;

namespace Resourcerer.Dtos.Entity;

public abstract class EntityDto<T> : IDto
{
    public Guid Id { get; set; }
    public eEntityStatus EntityStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid ModifiedBy { get; set; }
}
