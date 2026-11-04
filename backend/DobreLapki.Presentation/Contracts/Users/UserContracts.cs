using System.ComponentModel.DataAnnotations;

namespace DobreLapki.Presentation.Contracts.Users;

/// <summary>Пользователь, отдаваемый клиенту (без пароля).</summary>
public sealed class UserDto
{
    /// <summary>Идентификатор.</summary>
    public int Id { get; init; }

    /// <summary>Имя.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Email.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Телефон.</summary>
    public string Phone { get; init; } = string.Empty;

    /// <summary>Ссылка на аватар.</summary>
    public string Avatar { get; init; } = string.Empty;
}

/// <summary>Запрос на регистрацию.</summary>
public sealed class RegisterUserRequest
{
    /// <summary>Имя (2–100 символов).</summary>
    [Required(ErrorMessage = "Имя обязательно.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Email.</summary>
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Введите корректный email.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Телефон (необязательно).</summary>
    [StringLength(30)]
    [RegularExpression("^[+()\\d\\s-]*$", ErrorMessage = "Недопустимые символы в телефоне.")]
    public string? Phone { get; set; }

    /// <summary>Пароль (6–100 символов).</summary>
    [Required(ErrorMessage = "Пароль обязателен.")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Запрос на вход.</summary>
public sealed class LoginRequest
{
    /// <summary>Email.</summary>
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Введите корректный email.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Пароль.</summary>
    [Required(ErrorMessage = "Пароль обязателен.")]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}

/// <summary>Запрос на обновление профиля.</summary>
public sealed class UpdateUserRequest
{
    /// <summary>Имя (2–100 символов).</summary>
    [Required(ErrorMessage = "Имя обязательно.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Email.</summary>
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Введите корректный email.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Телефон (необязательно).</summary>
    [StringLength(30)]
    [RegularExpression("^[+()\\d\\s-]*$", ErrorMessage = "Недопустимые символы в телефоне.")]
    public string? Phone { get; set; }
}

/// <summary>Параметры выборки пользователей.</summary>
public sealed class UserQuery
{
    /// <summary>Номер страницы.</summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>Размер страницы (1–100).</summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
