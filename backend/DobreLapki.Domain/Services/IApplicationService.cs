using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Models;

namespace DobreLapki.Domain.Services;

/// <summary>Доменный сервис заявок на усыновление.</summary>
public interface IApplicationService
{
    /// <summary>Возвращает страницу заявок.</summary>
    Task<PagedResult<AdoptionApplication>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Возвращает заявку по идентификатору или бросает NotFoundException.</summary>
    Task<AdoptionApplication> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Создаёт заявку после проверки доменных правил.</summary>
    Task<AdoptionApplication> CreateAsync(ApplicationDraft draft, CancellationToken cancellationToken);

    /// <summary>Изменяет статус заявки по доменным правилам.</summary>
    Task<AdoptionApplication> UpdateAsync(int id, ApplicationUpdateDraft draft, CancellationToken cancellationToken);

    /// <summary>Удаляет заявку или бросает NotFoundException.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
