using System.Net.Mail;
using System.Text.RegularExpressions;
using DobreLapki.Domain.Common;
using DobreLapki.Domain.Models;

namespace DobreLapki.Domain.Validation;

/// <summary>Доменные правила и проверки для заявки на усыновление.</summary>
public static partial class ApplicationRules
{
    [GeneratedRegex("^[+()\\d\\s-]+$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex("\\d")]
    private static partial Regex DigitRegex();

    /// <summary>Нормализует строки и проверяет поля заявки.</summary>
    public static ApplicationDraft Normalize(ApplicationDraft draft)
    {
        if (draft.AnimalId <= 0)
        {
            throw new DomainValidationException("Идентификатор животного должен быть положительным.");
        }

        var fullName = TextSanitizer.Clean(
            draft.FullName, "Имя и фамилия", 100, required: true, minLength: 2);
        var email = TextSanitizer.Clean(draft.Email, "Email", 200, required: true, minLength: 3)
            .ToLowerInvariant();
        var phone = TextSanitizer.Clean(draft.Phone, "Телефон", 30, required: true, minLength: 10);
        var about = TextSanitizer.Clean(
            draft.About, "Рассказ о себе", 1000, required: true, minLength: 10);

        if (!IsValidEmail(email))
        {
            throw new DomainValidationException("Введите корректный email.");
        }

        if (!PhoneRegex().IsMatch(phone) || DigitRegex().Matches(phone).Count < 10)
        {
            throw new DomainValidationException("Введите корректный номер телефона.");
        }

        return draft with
        {
            FullName = fullName,
            Email = email,
            Phone = phone,
            About = about
        };
    }

    internal static bool IsValidEmail(string email) =>
        MailAddress.TryCreate(email, out var address) &&
        string.Equals(address.Address, email, StringComparison.OrdinalIgnoreCase);
}
