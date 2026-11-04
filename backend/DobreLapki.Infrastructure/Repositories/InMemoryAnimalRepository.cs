using System.Globalization;
using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Filters;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Validation;
using DobreLapki.Infrastructure.Seed;

namespace DobreLapki.Infrastructure.Repositories;

/// <summary>
/// In-memory-реализация хранилища животных (заглушка БД).
/// Потокобезопасна за счёт lock; при появлении реальной БД заменяется без изменения контроллеров.
/// </summary>
public sealed class InMemoryAnimalRepository : IAnimalRepository
{
    private static readonly StringComparer NameComparer =
        StringComparer.Create(CultureInfo.GetCultureInfo("ru-RU"), ignoreCase: true);

    private readonly object _sync = new();
    private readonly Dictionary<int, Animal> _items;
    private int _nextId;

    /// <summary>Создаёт репозиторий с демонстрационными данными.</summary>
    public InMemoryAnimalRepository()
        : this(SeedData.CreateAnimals())
    {
    }

    private InMemoryAnimalRepository(IEnumerable<Animal> seed)
    {
        var animals = seed?.ToList() ?? new List<Animal>();
        _items = animals.ToDictionary(animal => animal.Id);
        _nextId = _items.Count == 0 ? 1 : _items.Keys.Max() + 1;
    }

    /// <summary>Создаёт репозиторий с переданным набором животных (для тестов).</summary>
    public static InMemoryAnimalRepository CreateWith(IEnumerable<Animal> seed) => new(seed);

    /// <inheritdoc />
    public Task<PagedResult<Animal>> GetPagedAsync(
        AnimalFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            IEnumerable<Animal> query = _items.Values;

            if (filter.Types.Count > 0)
            {
                query = query.Where(animal => filter.Types.Contains(animal.Type));
            }

            if (filter.Genders.Count > 0)
            {
                query = query.Where(animal => filter.Genders.Contains(animal.Gender));
            }

            if (filter.Ages.Count > 0)
            {
                query = query.Where(animal => filter.Ages.Contains(animal.AgeBucket));
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(animal =>
                    animal.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    animal.Breed.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var total = query.Count();
            var items = Sort(query, filter.Sort)
                .Skip(PaginationGuard.Skip(page, pageSize))
                .Take(pageSize)
                .ToList();

            return Task.FromResult(new PagedResult<Animal>(items, page, pageSize, total));
        }
    }

    /// <inheritdoc />
    public Task<Animal?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            _items.TryGetValue(id, out var animal);
            return Task.FromResult(animal);
        }
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Animal>> GetPopularAsync(int count, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            IReadOnlyList<Animal> items = _items.Values
                .OrderByDescending(animal => animal.ArrivedAt)
                .ThenByDescending(animal => animal.Id)
                .Take(count)
                .ToList();

            return Task.FromResult(items);
        }
    }

    /// <inheritdoc />
    public Task<bool> ExistsByNameAndTypeAsync(
        string name, AnimalType type, int? excludeId, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            var exists = _items.Values.Any(animal =>
                animal.Type == type &&
                (excludeId is null || animal.Id != excludeId) &&
                NameComparer.Equals(animal.Name, name));

            return Task.FromResult(exists);
        }
    }

    /// <inheritdoc />
    public Task<Animal> CreateAsync(Animal animal, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            animal.Id = _nextId++;
            _items[animal.Id] = animal;
            return Task.FromResult(animal);
        }
    }

    /// <inheritdoc />
    public Task<Animal?> UpdateAsync(Animal animal, CancellationToken cancellationToken)
    {
        lock (_sync)
        {
            if (!_items.ContainsKey(animal.Id))
            {
                return Task.FromResult<Animal?>(null);
            }

            _items[animal.Id] = animal;
            return Task.FromResult<Animal?>(animal);
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

    private static IEnumerable<Animal> Sort(IEnumerable<Animal> query, AnimalSort sort) => sort switch
    {
        AnimalSort.Oldest => query
            .OrderBy(animal => animal.ArrivedAt)
            .ThenBy(animal => animal.Id),
        AnimalSort.Name => query
            .OrderBy(animal => animal.Name, NameComparer)
            .ThenBy(animal => animal.Id),
        _ => query
            .OrderByDescending(animal => animal.ArrivedAt)
            .ThenByDescending(animal => animal.Id)
    };
}
