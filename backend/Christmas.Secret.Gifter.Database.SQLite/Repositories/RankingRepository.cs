using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using System.Threading;

namespace MSSql.Infrastructure.Repositories {
    internal class RankingRepository : IRankingRepository
    {
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<DataDbContext> _contextFactory;
        private readonly ILogger<RankingRepository> _logger;

        public RankingRepository(
            ILogger<RankingRepository> logger,
            IDbContextFactory<DataDbContext> _contextFactory,
            IMapper mapper)
        {
            _logger = logger;
            this._contextFactory = _contextFactory;
            _mapper = mapper;
        }

        public async Task<RankingEntry[]> GetAllAsync(string id, CancellationToken? cancellationToken) {
            try {
                cancellationToken?.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var allOfThem = await _context
                    .Ranks
                    .ToArrayAsync(cancellationToken ?? default);

                var mapped = allOfThem.Select(p => _mapper.Map<RankingEntry>(p));

                return mapped.ToArray();
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
            }

            return null;
        }

        public async Task<RankingEntry> AddAsync(RankingEntry item, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var result = await _context
                    .Ranks
                    .AddAsync(item, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                var mappedResult = _mapper.Map<RankingEntry>(result.Entity);

                return mappedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException("Adding failed", ex);
            }
        }

        public async Task<RankingEntry> UpdateAsync(RankingEntry item, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var existed = await _context
                   .Ranks
                   .AsNoTracking()
                   .AsQueryable()
                   .FirstOrDefaultAsync(p => p.Id == item.Id, cancellationToken);

                if (existed == null)
                {
                    throw new Exception("Entity not found");
                }

                var mapped = _mapper.Map(item, existed);
                var result = _context.Update(mapped);

                await _context.SaveChangesAsync(cancellationToken);

                return result.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException("Updating failed", ex);
            }
        }

        public async Task<int> DeleteAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var existed = await _context
                    .Ranks
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                var deleted = _context
                    .Ranks
                    .Remove(existed);

                var result = await _context.SaveChangesAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException("Deleting failed", ex);
            }
        }

        public async Task<RankingEntry> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var found = await _context
                    .Ranks
                    .AsQueryable()
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (found == null)
                {
                    return null;
                }

                var mappedResult = _mapper.Map<RankingEntry>(found);

                return mappedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }

        public async Task<RankingEntry> GetByAlgorithmId(string algorithmId, CancellationToken cancellationToken) {
            try {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var found = await _context
                    .Ranks
                    .Where(p => p.AlgorithmId == algorithmId)
                    .FirstOrDefaultAsync();

                var mappedResult = _mapper.Map<RankingEntry>(found);

                return mappedResult;
            } catch (Exception ex) {
                _logger.LogError(ex.Message);
            }

            return null;
        }
    }
}
