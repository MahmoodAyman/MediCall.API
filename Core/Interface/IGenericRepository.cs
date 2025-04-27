using System;
using System.Linq.Expressions;
using Core.Models;

namespace Core.Interface;

public interface IGenericRepository<T> where T : class, IDeleteable
{

    // General Crud Ops 
    Task<T?> GetByIdAsync(object id);
    Task<IReadOnlyList<T>> GetAllAsync();

    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T> AddAsync(T entity);

    T Update(T entity);

    Task<bool> DeleteByIdAsync(object id);
    Task<bool> IsUsedAsync(Expression<Func<T, bool>> predicate);
    Task<bool> IsIdValidTypeAsync<Type>(object id) where Type : class;

    // context.SaveChangesAsync(); 
    Task<bool> SaveAllAsync();


}
