namespace Domain.Common.Abstractions;

public interface IRepositoryBase<T> where T : class, IAggregateRoot
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<T?> GetByGuidIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);

    Task AddAsync(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
}
