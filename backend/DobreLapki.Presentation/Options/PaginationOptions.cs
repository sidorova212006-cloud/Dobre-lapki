using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace DobreLapki.Presentation.Options;

/// <summary>Настройки пагинации.</summary>
public sealed class PaginationOptions
{
    /// <summary>Имя секции конфигурации.</summary>
    public const string SectionName = "Pagination";

    /// <summary>Размер страницы по умолчанию.</summary>
    [Range(1, 100)]
    public int DefaultPageSize { get; set; } = 20;

    /// <summary>Максимально допустимый размер страницы.</summary>
    [Range(1, 100)]
    public int MaxPageSize { get; set; } = 100;
}

/// <summary>Перекрёстная проверка настроек пагинации.</summary>
public sealed class PaginationOptionsValidator : IValidateOptions<PaginationOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, PaginationOptions options)
    {
        if (options.DefaultPageSize > options.MaxPageSize)
        {
            return ValidateOptionsResult.Fail(
                "Pagination:DefaultPageSize не может быть больше Pagination:MaxPageSize.");
        }

        return ValidateOptionsResult.Success;
    }
}
