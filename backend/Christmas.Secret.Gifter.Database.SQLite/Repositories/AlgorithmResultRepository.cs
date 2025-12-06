using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;

namespace MSSql.Infrastructure.Repositories {
    internal class AlgorithmResultRepository : IAlgorithmResultRepository
    {
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<DataDbContext> _contextFactory;
        private readonly ILogger<AlgorithmResultRepository> _logger;

        public AlgorithmResultRepository(
            ILogger<AlgorithmResultRepository> logger,
            IDbContextFactory<DataDbContext> _contextFactory,
            IMapper mapper)
        {
            _logger = logger;
            this._contextFactory = _contextFactory;
            _mapper = mapper;
        }

        public async Task<AlgorithmResultEntry> AddAsync(AlgorithmResultEntry item, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var context = _contextFactory.CreateDbContext();

                var result = await context
                    .Results
                    .AddAsync(item, cancellationToken);

                await context.SaveChangesAsync(cancellationToken);

                var mappedResult = _mapper.Map<AlgorithmResultEntry>(result.Entity);

                return mappedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException("Adding failed", ex);
            }
        }

        public async Task<AlgorithmResultEntry> UpdateAsync(AlgorithmResultEntry item, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var context = _contextFactory.CreateDbContext();

                var existed = await context
                   .Results
                   .Include(p => p.Summary)
                   .AsQueryable()
                   .FirstOrDefaultAsync(p => p.Id == item.Id, cancellationToken);

                if (existed == null)
                {
                    throw new Exception("Entity not found");
                }

                var mapped = _mapper.Map(item, existed);
                var result = context.Update(mapped);

                await context.SaveChangesAsync(cancellationToken);

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
                using var context = _contextFactory.CreateDbContext();

                var existed = await context
                    .Results
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                var deleted = context
                    .Results
                    .Remove(existed);

                var result = await context.SaveChangesAsync(cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException("Deleting failed", ex);
            }
        }

        public async Task<AlgorithmResultEntry> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var context = _contextFactory.CreateDbContext();

                var found = await context
                    .Results
                    .Include(p => p.Summary)
                    .AsQueryable()
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (found == null)
                {
                    return null;
                }

                var mappedResult = _mapper.Map<AlgorithmResultEntry>(found);

                return mappedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }
    }
}
