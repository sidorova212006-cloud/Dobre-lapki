using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Models;

namespace DobreLapki.Domain.Services;

/// <summary>Доменный сервис пользователей.</summary>
public interface IUserService
{
    /// <summary>Возвращает страницу пользователей.</summary>
    Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Возвращает пользователя по идентификатору или бросает NotFoundException.</summary>
    Task<User> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Регистрирует пользователя после проверки доменных правил.</summary>
    Task<User> RegisterAsync(UserRegistration draft, CancellationToken cancellationToken);

    /// <summary>Проверяет учётные данные и возвращает пользователя.</summary>
    Task<User> LoginAsync(UserLogin draft, CancellationToken cancellationToken);

    /// <summary>Обновляет профиль пользователя.</summary>
    Task<User> UpdateAsync(int id, UserUpdateDraft draft, CancellationToken cancellationToken);

    /// <summary>Удаляет пользователя или бросает NotFoundException.</summary>
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
