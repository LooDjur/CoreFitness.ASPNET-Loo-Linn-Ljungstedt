using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Abstractions;

public interface IRepositoryBase<T> where T : class
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}
