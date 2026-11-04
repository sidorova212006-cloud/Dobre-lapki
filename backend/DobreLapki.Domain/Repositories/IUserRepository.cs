using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;

namespace DobreLapki.Domain.Repositories;

/// <summary>Хранилище пользователей.</summary>
public interface IUserRepository
{
    /// <summary>Возвращает страницу пользователей.</summary>
    Task<PagedResult<User>> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken);

    /// <summary>Возвращает пользователя по идентификатору или null.</summary>
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>Возвращает пользователя по email или null.</summary>
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>Создаёт пользователя и возвращает его с присвоенным идентификатором.</summary>
    Task<User> CreateAsync(User user, CancellationToken cancellationToken);

    /// <summary>Обновляет пользователя. Возвращает null, если его нет.</summary>
    Task<User?> UpdateAsync(User user, CancellationToken cancellationToken);

    /// <summary>Удаляет пользователя. Возвращает false, если его нет.</summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
