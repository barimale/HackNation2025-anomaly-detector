using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.Repositories.Abstractions {
    public interface IAlgorithmSummaryRepository : IBaseRepository<AlgorithmSummaryEntry, string> {
        Task<AlgorithmSummaryEntry[]> GetAllAsync(string eventId, CancellationToken? cancellationToken = null);
    }
}
