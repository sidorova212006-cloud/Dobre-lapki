using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Validation;
using DobreLapki.Infrastructure.Seed;

namespace DobreLapki.Infrastructure.Repositories;

/// <summary>In-memory-реализация хранилища заявок (заглушка БД).</summary>
public sealed class InMemoryApplicationRepository : IApplicationRepository
{
    private readonly object _sync = new();
    private readonly Dictionary<int, AdoptionApplication> _items;
    private int _nextId;

    /// <summary>Создаёт репозиторий с демонстрационными заявками.</summary>
    public InMemoryApplicationRepository()
        : this(SeedData.CreateApplications(SeedData.CreateAnimals()))
    {
    }

    private InMemoryApplicationRepository(IEnumerable<AdoptionApplication> seed)
    {
        var applications = seed?.ToList() ?? new List<AdoptionApplication>();
        _items = applications.ToDictionary(application => application.Id);
        _nextId = _items.Count == 0 ? 1 : _items.Keys.Max() + 1;
    }

    /// <summary>Создаёт репозиторий с переданным набором заявок (для тестов).</summary>
    public static InMemoryApplicationRepository CreateWith(IEnumerable<AdoptionApplication> seed) =>
        new(seed);

    /// <inheritdoc />
    public Task<PagedResult<AdoptionApplication>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            var total = _items.Count;
            IReadOnlyList<AdoptionApplication> items = _items.Values
                .OrderByDescending(application => application.CreatedAt)
                .ThenByDescending(application => application.Id)
                .Skip(PaginationGuard.Skip(page, pageSize))
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<AdoptionApplication>(items, page, pageSize, total));
        }
    }

    /// <inheritdoc />
    public Task<AdoptionApplication?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            _items.TryGetValue(id, out var application);
            return Task.FromResult(application);
        }
    }

    /// <inheritdoc />
    public Task<bool> ExistsPendingAsync(
        int animalId, string email, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            var exists = _items.Values.Any(application =>
                application.AnimalId == animalId &&
                application.Status == ApplicationStatus.Pending &&
                string.Equals(application.Email, email, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(exists);
        }
    }

    /// <inheritdoc />
    public Task<AdoptionApplication> CreateAsync(
        AdoptionApplication application, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            application.Id = _nextId++;
            _items[application.Id] = application;
            return Task.FromResult(application);
        }
    }

    /// <inheritdoc />
    public Task<AdoptionApplication?> UpdateAsync(
        AdoptionApplication application, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            if (!_items.ContainsKey(application.Id))
            {
                return Task.FromResult<AdoptionApplication?>(null);
            }

            _items[application.Id] = application;
            return Task.FromResult<AdoptionApplication?>(application);
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
