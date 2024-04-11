using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static T Map<T, TDto>(Func<T> generator, TDto src)
        where T : AppDbEntity
        where TDto : EntityDto<TDto>
    {
        var dest = generator();
        
        dest.Id = src.Id;
        dest.EntityStatus = src.EntityStatus;
        dest.CreatedAt = src.CreatedAt;
        dest.CreatedBy = src.CreatedBy;
        dest.ModifiedAt = src.ModifiedAt;
        dest.ModifiedBy = src.ModifiedBy;
        
        return dest;
    }

    public static T? Map<T, TDto>(TDto? src, Func<TDto, T> mapper)
        where T : AppDbEntity
        where TDto : EntityDto<TDto>
        => src == null ? default : mapper(src);

    public static TDto Map<T, TDto>(Func<TDto> generator, T src)
        where T : AppDbEntity
        where TDto : EntityDto<TDto>
    {
        var dest = generator();
        
        dest.Id = src.Id;
        dest.EntityStatus = src.EntityStatus;
        dest.CreatedAt = src.CreatedAt;
        dest.CreatedBy = src.CreatedBy;
        dest.ModifiedAt = src.ModifiedAt;
        dest.ModifiedBy = src.ModifiedBy;

        return dest;
    }

    public static TDto? Map<T, TDto>(T? src, Func<T, TDto> mapper)
        where T : AppDbEntity
        where TDto : EntityDto<TDto>
        => src == null ? default : mapper(src);
}
