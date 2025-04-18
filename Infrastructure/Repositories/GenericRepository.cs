using System;

namespace Infrastructure.Repositories;

using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Interface;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T>(MediCallContext _context) : IGenericRepository<T> where T : class
{
    private readonly DbSet<T> _dbSet = _context.Set<T>();
    public void Add(T entity)
    {
        _dbSet.Add(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public void Update(T entity)
    {
        _dbSet.Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;
    }
}
