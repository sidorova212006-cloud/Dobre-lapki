using DobreLapki.Domain.Common;

namespace DobreLapki.Domain.Repositories;

/// <summary>Хранилище избранных животных.</summary>
public interface IFavoritesRepository
{
    /// <summary>Возвращает страницу идентификаторов избранного.</summary>
    Task<PagedResult<int>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Проверяет, находится ли животное в избранном.</summary>
    Task<bool> ExistsAsync(int animalId, CancellationToken cancellationToken);

    /// <summary>Добавляет животное в избранное. False — уже есть.</summary>
    Task<bool> AddAsync(int animalId, CancellationToken cancellationToken);

    /// <summary>Удаляет животное из избранного. False — его там нет.</summary>
    Task<bool> RemoveAsync(int animalId, CancellationToken cancellationToken);
}
