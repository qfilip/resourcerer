using Resourcerer.DataAccess.Entities;
using Resourcerer.Dtos.Entity;

namespace Resourcerer.Logic.Utilities.Map;

public static class Categories
{
    public static Category[] MapTree(IEnumerable<Category> categories)
    {
        Category MapChildren(Category current)
        {
            var children = categories
                .Where(x => x.ParentCategoryId == current.Id)
                .ToArray();

            current.ChildCategories = children
                .Select(MapChildren)
                .ToArray();

            return current;
        }

        return categories
            .Where(x => x.ParentCategoryId == null)
            .Select(MapChildren)
            .ToArray();
    }

    public static CategoryDto[] MapTree(IEnumerable<CategoryDto> categories)
    {
        CategoryDto MapChildren(CategoryDto current)
        {
            var children = categories
                .Where(x => x.ParentCategoryId == current.Id)
                .ToArray();

            current.ChildCategories = children
                .Select(MapChildren)
                .ToArray();

            return current;
        }

        return categories
            .Where(x => x.ParentCategoryId == null)
            .Select(MapChildren)
            .ToArray();
    }
}
