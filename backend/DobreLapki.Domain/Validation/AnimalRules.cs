using DobreLapki.Domain.Common;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Models;

namespace DobreLapki.Domain.Validation;

/// <summary>Доменные правила и проверки для животного.</summary>
public static class AnimalRules
{
    /// <summary>Нормализует строки и проверяет все поля животного.</summary>
    public static AnimalDraft Normalize(AnimalDraft draft)
    {
        var name = TextSanitizer.Clean(draft.Name, "Кличка", 100, required: true, minLength: 1);
        var breed = TextSanitizer.Clean(draft.Breed, "Порода", 100, required: true, minLength: 1);
        var description = TextSanitizer.Clean(
            draft.Description, "Описание", 2000, required: true, minLength: 10);
        var health = TextSanitizer.Clean(draft.Health, "Здоровье", 200, required: true, minLength: 1);

        EnsureDefined(draft.Type, "Вид животного");
        EnsureDefined(draft.Gender, "Пол животного");

        if (double.IsNaN(draft.AgeYears) || double.IsInfinity(draft.AgeYears))
        {
            throw new DomainValidationException("Возраст должен быть числом.");
        }

        if (draft.AgeYears < 0 || draft.AgeYears > 50)
        {
            throw new DomainValidationException("Возраст должен быть от 0 до 50 лет.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        if (draft.ArrivedAt > today)
        {
            throw new DomainValidationException("Дата поступления не может быть в будущем.");
        }

        if (draft.ArrivedAt < new DateOnly(1990, 1, 1))
        {
            throw new DomainValidationException("Дата поступления не может быть раньше 1990 года.");
        }

        return draft with
        {
            Name = name,
            Breed = breed,
            Description = description,
            Health = health,
            Photos = NormalizePhotos(draft.Photos)
        };
    }

    private static IReadOnlyList<string> NormalizePhotos(IReadOnlyList<string>? photos)
    {
        if (photos is null || photos.Count == 0)
        {
            return Array.Empty<string>();
        }

        if (photos.Count > 10)
        {
            throw new DomainValidationException("Количество фотографий не должно превышать 10.");
        }

        var result = new List<string>(photos.Count);
        foreach (var photo in photos)
        {
            var url = TextSanitizer.Clean(photo, "Ссылка на фото", 500, required: true, minLength: 1);
            if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new DomainValidationException(
                    "Ссылка на фото должна быть абсолютным http(s)-адресом.");
            }

            result.Add(url);
        }

        return result;
    }

    private static void EnsureDefined<TEnum>(TEnum value, string fieldName)
        where TEnum : struct, Enum
    {
        if (!Enum.IsDefined(value))
        {
            throw new DomainValidationException($"Поле «{fieldName}» содержит недопустимое значение.");
        }
    }
}
