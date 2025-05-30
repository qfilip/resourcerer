﻿using Resourcerer.DataAccess.Abstractions;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos.Records;

namespace Resourcerer.Dtos.Entities;

public class EntityDto : IDto, IId<Guid>, IAuditedEntity<AuditDto>, ISoftDeletable
{
    public Guid Id { get; set; }
    public eEntityStatus EntityStatus { get; set; }
    public AuditDto AuditRecord { get; set; } = new();
}
