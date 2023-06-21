﻿namespace Resourcerer.Dtos;

public class EntityDto : DtoBase
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}
