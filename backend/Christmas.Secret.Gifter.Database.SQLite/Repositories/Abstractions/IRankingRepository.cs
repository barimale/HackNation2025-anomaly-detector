using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.Repositories.Abstractions {
    public interface IRankingRepository : IBaseRepository<RankingEntry, string> {
        public Task<RankingEntry> GetByAlgorithmId(string algorithmId, CancellationToken cancellationToken);
        public Task<RankingEntry[]> GetAllAsync(string id, CancellationToken? cancellationToken);
        }
}
