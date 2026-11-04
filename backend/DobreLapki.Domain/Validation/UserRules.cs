using DobreLapki.Domain.Common;
using DobreLapki.Domain.Models;

namespace DobreLapki.Domain.Validation;

/// <summary>Доменные правила и проверки для пользователя.</summary>
public static class UserRules
{
    /// <summary>Нормализует и проверяет данные регистрации.</summary>
    public static UserRegistration NormalizeRegistration(UserRegistration draft)
    {
        var name = TextSanitizer.Clean(draft.Name, "Имя", 100, required: true, minLength: 2);
        var email = NormalizeEmail(draft.Email);
        var phone = NormalizePhone(draft.Phone);

        if (string.IsNullOrWhiteSpace(draft.Password))
        {
            throw new DomainValidationException("Пароль обязателен.");
        }

        if (draft.Password.Length < 6 || draft.Password.Length > 100)
        {
            throw new DomainValidationException("Пароль должен содержать от 6 до 100 символов.");
        }

        return draft with { Name = name, Email = email, Phone = phone };
    }

    /// <summary>Нормализует данные входа.</summary>
    public static UserLogin NormalizeLogin(UserLogin draft)
    {
        var email = NormalizeEmail(draft.Email);
        if (string.IsNullOrWhiteSpace(draft.Password))
        {
            throw new DomainValidationException("Пароль обязателен.");
        }

        return draft with { Email = email };
    }

    /// <summary>Нормализует данные обновления профиля.</summary>
    public static UserUpdateDraft NormalizeUpdate(UserUpdateDraft draft)
    {
        var name = TextSanitizer.Clean(draft.Name, "Имя", 100, required: true, minLength: 2);
        var email = NormalizeEmail(draft.Email);
        var phone = NormalizePhone(draft.Phone);
        return draft with { Name = name, Email = email, Phone = phone };
    }

    private static string NormalizeEmail(string? value)
    {
        var email = TextSanitizer.Clean(value, "Email", 200, required: true, minLength: 3)
            .ToLowerInvariant();
        if (!ApplicationRules.IsValidEmail(email))
        {
            throw new DomainValidationException("Введите корректный email.");
        }

        return email;
    }

    private static string NormalizePhone(string? value)
    {
        var phone = TextSanitizer.Clean(value, "Телефон", 30, required: false);
        if (phone.Length > 0 && !phone.All(c => char.IsDigit(c) || c is '+' or '(' or ')' or '-' or ' '))
        {
            throw new DomainValidationException("Телефон содержит недопустимые символы.");
        }

        return phone;
    }
}
