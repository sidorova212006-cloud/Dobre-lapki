using DobreLapki.Domain.Common;

namespace DobreLapki.Domain.Services;

/// <summary>Доменный сервис избранного.</summary>
public interface IFavoriteService
{
    /// <summary>Возвращает страницу идентификаторов избранного.</summary>
    Task<PagedResult<int>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Добавляет животное в избранное.</summary>
    Task AddAsync(int animalId, CancellationToken cancellationToken);

    /// <summary>Удаляет животное из избранного.</summary>
    Task RemoveAsync(int animalId, CancellationToken cancellationToken);
}
