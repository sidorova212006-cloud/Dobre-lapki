namespace DobreLapki.Domain.Entities;

/// <summary>Пользователь приюта.</summary>
public sealed class User
{
    /// <summary>Идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Имя.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Email (уникальный, в нижнем регистре).</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Телефон.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Ссылка на аватар.</summary>
    public string Avatar { get; set; } = string.Empty;

    /// <summary>Хеш пароля (никогда не отдаётся клиенту).</summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>Соль пароля (никогда не отдаётся клиенту).</summary>
    public string PasswordSalt { get; set; } = string.Empty;
}
