﻿using Resourcerer.DataAccess.Enums;

namespace Resourcerer.DataAccess.Entities;
public class EntityBase
{
    public Guid Id { get; set; }
    public eEntityStatus EntityStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
