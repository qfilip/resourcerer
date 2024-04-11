using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic;

public static partial class Mapper
{
    public static Category Map(CategoryDto src) =>
        Map(() =>
            new Category
            {
                Name = src.Name,
                CompanyId = src.CompanyId,
                Company = Map(src.Company, Map),
                ParentCategoryId = src.ParentCategoryId,
                ParentCategory = Map(src.ParentCategory, Map),
                ChildCategories = src.ChildCategories.Select(cc => Map(cc)).ToArray(),
                Items = src.Items.Select(i => Map(i)).ToArray()
            }, src);

    public static CategoryDto Map(Category src) =>
        Map(() =>
            new CategoryDto
            {
                Name = src.Name,
                CompanyId = src.CompanyId,
                Company = Map(src.Company, Map),
                ParentCategoryId = src.ParentCategoryId,
                ParentCategory = Map(src.ParentCategory, Map),
                ChildCategories = src.ChildCategories.Select(cc => Map(cc)).ToArray(),
                Items = src.Items.Select(i => Map(i)).ToArray()
            }, src);
}
