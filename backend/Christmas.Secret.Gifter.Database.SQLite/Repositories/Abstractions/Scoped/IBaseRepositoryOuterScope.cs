namespace MSSql.Infrastructure.Repositories.Abstractions.Scoped {
    public interface IBaseRepositoryOuterScope<T, U>
        where T : class
        where U : class
    {
        Task<T> AddAsync(T item, CancellationToken cancellationToken = default);
        Task<T> GetByIdAsync(U id, CancellationToken cancellationToken = default);
    }
}
