﻿using Microsoft.EntityFrameworkCore;
using Resourcerer.Application.Abstractions.Services;
using Resourcerer.DataAccess.Contexts;
using Resourcerer.DataAccess.Entities;

namespace Resourcerer.UnitTests.Utilities;

public class TestDbContext : AppDbContext
{
    public TestDbContext(
        DbContextOptions<AppDbContext> options,
        IAppIdentityService<AppUser> _appIdentityService) : base(options, _appIdentityService)
    {
    }

    public void Clear() => ChangeTracker.Clear();

    public override int SaveChanges()
    {
        var result = base.SaveChanges();
        base.ChangeTracker.Clear();

        return result;
    }
}
