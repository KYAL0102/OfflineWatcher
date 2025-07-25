﻿namespace Base.Persistence;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Base.Core.Contracts;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class, IEntityObject
{
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(DbContext context)
    {
        Context = context;
        _dbSet = context.Set<TEntity>();
    }

    public DbContext  Context { get; }
    protected DbSet<TEntity> DbSet => _dbSet;

    #region GetAsync

    public async Task<IList<TEntity>> GetNoTrackingAsync(
        Expression<Func<TEntity, bool>>?  filter  = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params string[] includeProperties)
    {
        return await GetAsync(_dbSet.AsNoTracking(), filter, orderBy, includeProperties);
    }

    public async Task<IList<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>?  filter  = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params string[] includeProperties)
    {
        return await GetAsync(_dbSet, filter, orderBy, includeProperties);
    }

    public async Task<IList<TEntity>> GetAsync(
        IQueryable<TEntity> query,
        Expression<Func<TEntity, bool>>? filter  = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        params string[] includeProperties)
    {
        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (string includeProperty in includeProperties!)
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return await orderBy(query).ToListAsync();
        }

        return await query.ToListAsync();
    }

    #endregion

    public async Task<TEntity?> GetByIdAsync(int id, params string[]? includeProperties)
    {
        if (includeProperties != null)
        {
            IQueryable<TEntity> query = _dbSet;
            foreach (string includeProperty in includeProperties!)
            {
                query = query.Include(includeProperty);
            }

            query = query.Where(e => e.Id == id);
            return await query.SingleOrDefaultAsync();
        }
        else
        {
            var findTask = await _dbSet.FindAsync(id);
            return findTask;
        }
    }

    public bool Exists(int id)
    {
        return DbSet.Any(e => e.Id == id);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await DbSet.AnyAsync(e => e.Id == id);
    }

    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public EntityEntry<TEntity> Attach(TEntity entity)
    {
        return _dbSet.Attach(entity);
    }

    public async Task<EntityEntry<TEntity>> AddAsync(TEntity entity)
    {
        return await _dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    public bool HasChanges()
    {
        return Context.ChangeTracker.HasChanges();
    }

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
    {
        IQueryable<TEntity> query = _dbSet;
        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync();
    }
}