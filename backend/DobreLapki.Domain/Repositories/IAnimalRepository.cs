using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Filters;

namespace DobreLapki.Domain.Repositories;

/// <summary>Хранилище животных. Реализация скрывает способ хранения данных.</summary>
public interface IAnimalRepository
{
    /// <summary>Возвращает страницу животных по фильтру.</summary>
    Task<PagedResult<Animal>> GetPagedAsync(AnimalFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Возвращает животное по идентификатору или null.</summary>
    Task<Animal?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Возвращает самых «свежих» животных.</summary>
    Task<IReadOnlyList<Animal>> GetPopularAsync(int count, CancellationToken cancellationToken);

    /// <summary>Проверяет, есть ли животное с такой кличкой и видом.</summary>
    Task<bool> ExistsByNameAndTypeAsync(string name, Enums.AnimalType type, int? excludeId, CancellationToken cancellationToken);

    /// <summary>Создаёт животное и возвращает его с присвоенным идентификатором.</summary>
    Task<Animal> CreateAsync(Animal animal, CancellationToken cancellationToken);

    /// <summary>Обновляет животное. Возвращает null, если его нет.</summary>
    Task<Animal?> UpdateAsync(Animal animal, CancellationToken cancellationToken);

    /// <summary>Удаляет животное. Возвращает false, если его нет.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
