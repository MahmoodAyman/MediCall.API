using System;

namespace Infrastructure.Repositories;

using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
public class GenericRepository<T> : IGenericRepository<T> where T : class, IDeleteable
{
    private readonly MediCallContext _dbContext;
    private readonly DbSet<T> _dbSet;

    public GenericRepository(MediCallContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<T>();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity is not null && entity.IsDeleted)
        {
            entity = null;
        }

        return entity;
    }

    public async Task<T?> GetByIdAsync(object id1 , object id2)
    {
        var entity = await _dbSet.FindAsync(id1,id2);

        if (entity is not null && entity.IsDeleted)
        {
            entity = null;
        }

        return entity;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.Where(e => e.IsDeleted==false).AsNoTracking().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(e => e.IsDeleted==false).Where(predicate).ToListAsync();

    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public T Update(T entity)
    {
        _dbSet.Update(entity);
        return entity;
    }

    public async Task<bool> DeleteByIdAsync(object id)
    {
        var entity = await GetByIdAsync(id);
        if(entity == null)
        {
            return false;
        }
        entity.IsDeleted = true;
        _dbSet.Update(entity);
        return true;
    }

    public async Task<T> RestoreAsync(Expression<Func<T, bool>> predicate)
    {
        var entity = await _dbSet.SingleOrDefaultAsync(predicate) ?? throw new SqlNullValueException("This entity Not Found");
        if (!entity.IsDeleted)
        {
            throw new Exception("This entity is already active");
        }

        entity.IsDeleted = false;
        return entity;
    }

    public async Task<bool> IsUsedAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<bool> IsIdValidTypeAsync<Type>(object id) where Type : class
    {
        var entity = await _dbContext.Set<Type>().FindAsync(id);
        return entity == null ? throw new InvalidOperationException($"This {typeof(Type).Name} is not existed") : true;
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
