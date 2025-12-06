using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;

namespace MSSql.Infrastructure.Repositories {
    public class AlgorithmSummaryRepository : IAlgorithmSummaryRepository
    {
        private readonly IMapper _mapper;
        private readonly IDbContextFactory<DataDbContext> _contextFactory;
        private readonly ILogger<AlgorithmSummaryRepository> _logger;

        public AlgorithmSummaryRepository(
            ILogger<AlgorithmSummaryRepository> logger,
            IDbContextFactory<DataDbContext> _contextFactory,
            IMapper mapper)
        {
            _logger = logger;
            this._contextFactory = _contextFactory;
            _mapper = mapper;
        }

        public async Task<AlgorithmSummaryEntry> AddAsync(AlgorithmSummaryEntry item, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var added = await _context.Summaries.AddAsync(item, cancellationToken);

                await _context.SaveChangesAsync(cancellationToken);

                return added.Entity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SystemException("Adding failed", ex);
            }
        }

        public async Task<AlgorithmSummaryEntry> UpdateAsync(AlgorithmSummaryEntry item, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var existed = await _context
                   .Summaries
                   .AsQueryable()
                   .FirstOrDefaultAsync(p => p.Id == item.Id, cancellationToken);

                if (existed == null)
                {
                    throw new Exception("Entity not found");
                }

                existed.VotedResult = item.VotedResult;
                existed.Results = item.Results;

                await _context.SaveChangesAsync(cancellationToken);

                var result = _context
                    .Update(existed);

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
                    .Summaries
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                var deleted = _context
                    .Summaries
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

        public async Task<AlgorithmSummaryEntry> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var found = await _context
                    .Summaries
                    .Include(p => p.Results)
                    .AsQueryable()
                    .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

                if (found == null)
                {
                    return null;
                }

                var mappedResult = _mapper.Map<AlgorithmSummaryEntry>(found);

                return mappedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }

        public async Task<AlgorithmSummaryEntry[]> GetAllAsync(string eventId, CancellationToken? cancellationToken)
        {
            try
            {
                cancellationToken?.ThrowIfCancellationRequested();
                using var _context = _contextFactory.CreateDbContext();

                var allOfThem = await _context
                    .Summaries
                    .Include(p => p.Results)//WIP
                    .ToArrayAsync(cancellationToken ?? default);

                var mapped = allOfThem.Select(p => _mapper.Map<AlgorithmSummaryEntry>(p));

                return mapped.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }

            return null;
        }
    }
}
