using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Filters;
using DobreLapki.Domain.Models;

namespace DobreLapki.Domain.Services;

/// <summary>Доменный сервис животных: правила, проверки, работа с хранилищем.</summary>
public interface IAnimalService
{
    /// <summary>Возвращает страницу животных по фильтру.</summary>
    Task<PagedResult<Animal>> GetPagedAsync(AnimalFilter filter, int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Возвращает животное по идентификатору или бросает NotFoundException.</summary>
    Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Возвращает популярных (самых новых) животных.</summary>
    Task<IReadOnlyList<Animal>> GetPopularAsync(int count, CancellationToken cancellationToken);

    /// <summary>Создаёт животное после проверки доменных правил.</summary>
    Task<Animal> CreateAsync(AnimalDraft draft, CancellationToken cancellationToken);

    /// <summary>Обновляет животное после проверки доменных правил.</summary>
    Task<Animal> UpdateAsync(int id, AnimalDraft draft, CancellationToken cancellationToken);

    /// <summary>Удаляет животное или бросает NotFoundException.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
