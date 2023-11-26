using FluentValidation;

namespace Resourcerer.Dtos;

public class SetUserPermissionsDto : IBaseDto
{
    public Guid UserId { get; set; }
    public Dictionary<string, int>? Permissions { get; set; }

    
}
