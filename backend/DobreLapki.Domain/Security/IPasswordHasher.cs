namespace DobreLapki.Domain.Security;

/// <summary>Хеширование и проверка паролей.</summary>
public interface IPasswordHasher
{
    /// <summary>Создаёт хеш и соль для пароля.</summary>
    (string Hash, string Salt) HashPassword(string password);

    /// <summary>Проверяет пароль по сохранённым хешу и соли.</summary>
    bool Verify(string password, string hash, string salt);
}
