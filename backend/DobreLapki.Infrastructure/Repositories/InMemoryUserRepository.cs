using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Security;
using DobreLapki.Domain.Validation;
using DobreLapki.Infrastructure.Seed;

namespace DobreLapki.Infrastructure.Repositories;

/// <summary>In-memory-реализация хранилища пользователей (заглушка БД).</summary>
public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly object _sync = new();
    private readonly Dictionary<int, User> _items;
    private int _nextId;

    /// <summary>Создаёт репозиторий с демонстрационным пользователем.</summary>
    public InMemoryUserRepository(IPasswordHasher passwordHasher)
        : this(new[] { SeedData.CreateDemoUser(passwordHasher) })
    {
    }

    private InMemoryUserRepository(IEnumerable<User> seed)
    {
        var users = seed?.ToList() ?? new List<User>();
        _items = users.ToDictionary(user => user.Id);
        _nextId = _items.Count == 0 ? 1 : _items.Keys.Max() + 1;
    }

    /// <summary>Создаёт репозиторий с переданным набором пользователей (для тестов).</summary>
    public static InMemoryUserRepository CreateWith(IEnumerable<User> seed) => new(seed);

    /// <inheritdoc />
    public Task<PagedResult<User>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            var total = _items.Count;
            IReadOnlyList<User> items = _items.Values
                .OrderBy(user => user.Id)
                .Skip(PaginationGuard.Skip(page, pageSize))
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<User>(items, page, pageSize, total));
        }
    }

    /// <inheritdoc />
    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            _items.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }
    }

    /// <inheritdoc />
    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            var user = _items.Values.FirstOrDefault(item =>
                string.Equals(item.Email, email, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(user);
        }
    }

    /// <inheritdoc />
    public Task<User> CreateAsync(User user, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            user.Id = _nextId++;
            _items[user.Id] = user;
            return Task.FromResult(user);
        }
    }

    /// <inheritdoc />
    public Task<User?> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            if (!_items.ContainsKey(user.Id))
            {
                return Task.FromResult<User?>(null);
            }

            _items[user.Id] = user;
            return Task.FromResult<User?>(user);
        }
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            return Task.FromResult(_items.Remove(id));
        }
    }
}
