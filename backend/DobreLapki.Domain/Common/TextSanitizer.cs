using System.Text.RegularExpressions;

namespace DobreLapki.Domain.Common;

/// <summary>
/// Очистка пользовательских строк: обрезка, удаление управляющих символов и HTML-тегов.
/// Защищает от XSS и «мусорных» данных, которые попадают на фронтенд.
/// </summary>
public static partial class TextSanitizer
{
    [GeneratedRegex("<[^>]*>")]
    private static partial Regex HtmlTagRegex();

    [GeneratedRegex("[\\u0000-\\u001F\\u007F]")]
    private static partial Regex ControlCharsRegex();

    [GeneratedRegex("\\s+")]
    private static partial Regex WhitespaceRegex();

    /// <summary>
    /// Нормализует строку и проверяет её длину.
    /// </summary>
    /// <param name="value">Исходное значение (может быть null).</param>
    /// <param name="fieldName">Название поля для сообщения об ошибке.</param>
    /// <param name="maxLength">Максимальная длина.</param>
    /// <param name="required">Обязательно ли поле.</param>
    /// <param name="minLength">Минимальная длина непустого значения.</param>
    public static string Clean(
        string? value,
        string fieldName,
        int maxLength,
        bool required = true,
        int minLength = 0)
    {
        var raw = value ?? string.Empty;

        if (required && string.IsNullOrWhiteSpace(raw))
        {
            throw new DomainValidationException($"Поле «{fieldName}» обязательно.");
        }

        var cleaned = HtmlTagRegex().Replace(raw, string.Empty);
        cleaned = ControlCharsRegex().Replace(cleaned, string.Empty);
        cleaned = WhitespaceRegex().Replace(cleaned, " ").Trim();

        if (required && cleaned.Length == 0)
        {
            throw new DomainValidationException($"Поле «{fieldName}» обязательно.");
        }

        if (cleaned.Length > maxLength)
        {
            throw new DomainValidationException(
                $"Поле «{fieldName}» не должно превышать {maxLength} символов.");
        }

        if (cleaned.Length > 0 && cleaned.Length < minLength)
        {
            throw new DomainValidationException(
                $"Поле «{fieldName}» должно содержать минимум {minLength} символов.");
        }

        return cleaned;
    }
}
