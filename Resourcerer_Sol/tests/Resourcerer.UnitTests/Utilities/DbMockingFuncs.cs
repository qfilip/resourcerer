﻿using Resourcerer.DataAccess.Entities;
using Resourcerer.DataAccess.Enums;
using Resourcerer.Dtos;
using Resourcerer.Utilities.Cryptography;
using System.Text.Json;

namespace Resourcerer.Logic.Queries.Mocks;

public class DbMockingFuncs
{
    protected static DateTime Now = new DateTime(2000, 1, 1);
    protected static void MarkDeleted<T>(params T[] items) where T : EntityBase
    {
        foreach (var i in items)
            i.EntityStatus = eEntityStatus.Deleted;
    }
    protected static T MakeEntity<T>(Func<T> retn) where T : EntityBase
    {
        var e = retn();
        e.Id = Guid.NewGuid();
        e.CreatedAt = Now;
        e.ModifiedAt = Now;

        return e;
    }

    protected static AppUser MakeUser(string name, string password, bool allPermissions, Dictionary<string, string>? permissions = null)
    {
        var userPermissions = allPermissions ?
            Permission.GetAllPermissionsDictionary() :
            (permissions ?? new Dictionary<string, string>());

        return MakeEntity(() => new AppUser
        {
            Name = name,
            PasswordHash = Hasher.GetSha256Hash(password),
            Permissions = JsonSerializer.Serialize(userPermissions)
        });
    }

    protected static Category MakeCategory(string name, Category? parentCategory = null)
    {
        return MakeEntity(() => new Category
        {
            Name = name,
            ParentCategoryId = parentCategory?.Id
        });
    }

    protected static UnitOfMeasure MakeUnitOfMeasure(string name, string symbol)
    {
        return MakeEntity(() => new UnitOfMeasure
        {
            Name = name,
            Symbol = symbol
        });
    }

    protected static Item MakeElement(string name, Category category, UnitOfMeasure uom)
    {
        return MakeEntity(() => new Item
        {
            Name = name,
            CategoryId = category.Id,
            UnitOfMeasureId = uom.Id
        });
    }

    protected static Price MakePrice(double value, Item item)
    {
        return MakeEntity(() => new Price
        {
            UnitValue = value,
            ItemId = item.Id
        });
    }

    protected static IEnumerable<Excerpt> MakeExcerpts(Item composite, List<(Item, double)> ingredients)
    {
        return ingredients.Select(x =>
        {
            return MakeEntity(() => new Excerpt
            {
                CompositeId = composite.Id,
                //ElementId = x.Item1.Id,
                Quantity = x.Item2
            });
        });
    }
}
