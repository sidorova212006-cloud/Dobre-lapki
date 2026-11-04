using DobreLapki.Domain.Common;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Infrastructure.Repositories;

/// <summary>In-memory-реализация хранилища избранного (заглушка БД).</summary>
public sealed class InMemoryFavoritesRepository : IFavoritesRepository
{
    private readonly object _sync = new();
    private readonly List<int> _ids;

    /// <summary>Создаёт пустое хранилище избранного.</summary>
    public InMemoryFavoritesRepository()
        : this(Array.Empty<int>())
    {
    }

    private InMemoryFavoritesRepository(IEnumerable<int> seed)
    {
        _ids = (seed ?? Array.Empty<int>()).Distinct().ToList();
    }

    /// <summary>Создаёт хранилище с переданным набором идентификаторов (для тестов).</summary>
    public static InMemoryFavoritesRepository CreateWith(IEnumerable<int> seed) => new(seed);

    /// <inheritdoc />
    public Task<PagedResult<int>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            var total = _ids.Count;
            IReadOnlyList<int> items = _ids
                .Skip(PaginationGuard.Skip(page, pageSize))
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<int>(items, page, pageSize, total));
        }
    }

    /// <inheritdoc />
    public Task<bool> ExistsAsync(int animalId, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            return Task.FromResult(_ids.Contains(animalId));
        }
    }

    /// <inheritdoc />
    public Task<bool> AddAsync(int animalId, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            if (_ids.Contains(animalId))
            {
                return Task.FromResult(false);
            }

            _ids.Add(animalId);
            return Task.FromResult(true);
        }
    }

    /// <inheritdoc />
    public Task<bool> RemoveAsync(int animalId, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            return Task.FromResult(_ids.Remove(animalId));
        }
    }
}
