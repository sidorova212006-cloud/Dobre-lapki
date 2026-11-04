using System.ComponentModel.DataAnnotations;

namespace DobreLapki.Presentation.Options;

/// <summary>Общие настройки приложения.</summary>
public sealed class AppOptions
{
    /// <summary>Имя секции конфигурации.</summary>
    public const string SectionName = "App";

    /// <summary>Отображаемое имя приложения.</summary>
    [Required(ErrorMessage = "ApplicationName обязателен.")]
    [StringLength(100, MinimumLength = 1)]
    public string ApplicationName { get; set; } = "Dobre Lapki API";

    /// <summary>Флаг включения Swagger вне окружения Development.</summary>
    public bool SwaggerEnabled { get; set; } = true;
}
