using Resourcerer.DataAccess.Enums;
using Resourcerer.DataAccess.Records;

namespace Resourcerer.Dtos.Entity;

public class EntityDto<T> : IDto
{
    public Guid Id { get; set; }
    public eEntityStatus EntityStatus { get; set; }
    public AuditRecord AuditRecord { get; set; } = new();
}
