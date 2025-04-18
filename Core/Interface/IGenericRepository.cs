using System;
using Core.Models;

namespace Core.Interface;

public interface IGenericRepository<T> where T : BaseEntity
{

    // General Crud Ops 
    Task<T?> GetByIdAsync(object id);
    Task<IReadOnlyList<T>> GetAllAsync();

    void Add(T entity);

    void Update(T entity);

    void Delete(T entity);


    // context.SaveChangesAsync(); 
    Task<bool> SaveAllAsync();


}
