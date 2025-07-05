namespace Base.Persistence;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Base.Core.Contracts;

using Microsoft.EntityFrameworkCore;

public class BaseUnitOfWork : IBaseUnitOfWork
{
    public DbContext BaseApplicationDbContext { get; init; }
    private bool _disposed;

    public BaseUnitOfWork(DbContext dbContext)
    {
        BaseApplicationDbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync() => await BaseApplicationDbContext.SaveChangesAsync();
    public async Task DeleteDatabaseAsync() => await BaseApplicationDbContext.Database.EnsureDeletedAsync();
    public async Task MigrateDatabaseAsync() => await BaseApplicationDbContext.Database.MigrateAsync();
    public async Task CreateDatabaseAsync() => await BaseApplicationDbContext.Database.EnsureCreatedAsync();

    #region Dispose

    public async ValueTask DisposeAsync()
    {
        await DisposeAsync(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsync(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                await BaseApplicationDbContext.DisposeAsync();
            }
        }

        _disposed = true;
    }

    public void Dispose()
    {
        BaseApplicationDbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    #endregion
}