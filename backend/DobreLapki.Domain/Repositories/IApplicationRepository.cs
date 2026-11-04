using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;

namespace DobreLapki.Domain.Repositories;

/// <summary>Хранилище заявок на усыновление.</summary>
public interface IApplicationRepository
{
    /// <summary>Возвращает страницу заявок, сначала новые.</summary>
    Task<PagedResult<AdoptionApplication>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Возвращает заявку по идентификатору или null.</summary>
    Task<AdoptionApplication?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Проверяет наличие заявки в статусе «на рассмотрении».</summary>
    Task<bool> ExistsPendingAsync(int animalId, string email, CancellationToken cancellationToken);

    /// <summary>Создаёт заявку и возвращает её с присвоенным идентификатором.</summary>
    Task<AdoptionApplication> CreateAsync(AdoptionApplication application, CancellationToken cancellationToken);

    /// <summary>Обновляет заявку. Возвращает null, если её нет.</summary>
    Task<AdoptionApplication?> UpdateAsync(AdoptionApplication application, CancellationToken cancellationToken);

    /// <summary>Удаляет заявку. Возвращает false, если её нет.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
