using System.Security.Cryptography;
using System.Text;
using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Models;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Security;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Domain.Services;

/// <summary>Реализация доменного сервиса пользователей.</summary>
public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHasher _passwordHasher;

    /// <summary>Создаёт сервис поверх репозитория и хешера паролей.</summary>
    public UserService(IUserRepository repository, IPasswordHasher passwordHasher)
    {
        _repository = repository;
        _passwordHasher = passwordHasher;
    }

    /// <inheritdoc />
    public Task<PagedResult<User>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        PaginationGuard.Validate(page, pageSize);
        return _repository.GetPagedAsync(page, pageSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("Идентификатор пользователя должен быть положительным.");
        }

        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Пользователь с идентификатором {id} не найден.");
    }

    /// <inheritdoc />
    public async Task<User> RegisterAsync(UserRegistration draft, CancellationToken cancellationToken)
    {
        var normalized = UserRules.NormalizeRegistration(draft);

        if (await _repository.GetByEmailAsync(normalized.Email, cancellationToken) is not null)
        {
            throw new ConflictException("Пользователь с таким email уже зарегистрирован.");
        }

        var (hash, salt) = _passwordHasher.HashPassword(normalized.Password);

        var user = new User
        {
            Name = normalized.Name,
            Email = normalized.Email,
            Phone = normalized.Phone ?? string.Empty,
            Avatar = BuildAvatar(normalized.Email),
            PasswordHash = hash,
            PasswordSalt = salt
        };

        return await _repository.CreateAsync(user, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<User> LoginAsync(UserLogin draft, CancellationToken cancellationToken)
    {
        var normalized = UserRules.NormalizeLogin(draft);
        var user = await _repository.GetByEmailAsync(normalized.Email, cancellationToken);

        if (user is null || !_passwordHasher.Verify(normalized.Password, user.PasswordHash, user.PasswordSalt))
        {
            throw new DomainValidationException("Неверный email или пароль.");
        }

        return user;
    }

    /// <inheritdoc />
    public async Task<User> UpdateAsync(
        int id, UserUpdateDraft draft, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);
        var normalized = UserRules.NormalizeUpdate(draft);

        var byEmail = await _repository.GetByEmailAsync(normalized.Email, cancellationToken);
        if (byEmail is not null && byEmail.Id != id)
        {
            throw new ConflictException("Пользователь с таким email уже зарегистрирован.");
        }

        existing.Name = normalized.Name;
        existing.Email = normalized.Email;
        existing.Phone = normalized.Phone ?? string.Empty;

        return await _repository.UpdateAsync(existing, cancellationToken)
            ?? throw new NotFoundException($"Пользователь с идентификатором {id} не найден.");
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("Идентификатор пользователя должен быть положительным.");
        }

        if (!await _repository.DeleteAsync(id, cancellationToken))
        {
            throw new NotFoundException($"Пользователь с идентификатором {id} не найден.");
        }
    }

    private static string BuildAvatar(string email)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(email));
        var seed = Convert.ToHexString(hash)[..12].ToLowerInvariant();
        return $"https://picsum.photos/seed/dp-{seed}/200/200";
    }
}
