using DobreLapki.Domain.Common;
using DobreLapki.Presentation.Options;

namespace DobreLapki.Presentation.Contracts.Common;

/// <summary>Разбор строковых query-параметров и нормализация пагинации.</summary>
public static class QueryParsing
{
    /// <summary>Разбирает список enum-значений через запятую.</summary>
    public static IReadOnlyList<TEnum> ParseEnumList<TEnum>(string? raw, string parameterName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return Array.Empty<TEnum>();
        }

        var result = new List<TEnum>();
        foreach (var part in raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (!Enum.TryParse<TEnum>(part, ignoreCase: true, out var value) || !Enum.IsDefined(value))
            {
                throw new DomainValidationException(
                    $"Недопустимое значение «{part}» в параметре «{parameterName}».");
            }

            if (!result.Contains(value))
            {
                result.Add(value);
            }
        }

        return result;
    }

    /// <summary>Разбирает одно enum-значение с запасным вариантом.</summary>
    public static TEnum ParseEnum<TEnum>(string? raw, TEnum fallback, string parameterName)
        where TEnum : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            return fallback;
        }

        if (!Enum.TryParse<TEnum>(raw.Trim(), ignoreCase: true, out var value) || !Enum.IsDefined(value))
        {
            throw new DomainValidationException(
                $"Недопустимое значение «{raw}» в параметре «{parameterName}».");
        }

        return value;
    }

    /// <summary>Приводит page/pageSize к допустимым значениям с учётом конфигурации.</summary>
    public static (int Page, int PageSize) NormalizePaging(
        PaginationOptions options, int page, int pageSize)
    {
        var safePage = page < 1 ? 1 : page;
        var safeSize = pageSize < 1
            ? options.DefaultPageSize
            : Math.Min(pageSize, options.MaxPageSize);

        return (safePage, safeSize);
    }
}
