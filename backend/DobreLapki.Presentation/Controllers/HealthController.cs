using DobreLapki.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DobreLapki.Presentation.Controllers;

/// <summary>Проверка доступности API.</summary>
[ApiController]
[Route("api/v1/health")]
[Produces("application/json")]
public sealed class HealthController : ControllerBase
{
    private readonly AppOptions _options;

    /// <summary>Создаёт контроллер с настройками приложения.</summary>
    public HealthController(IOptions<AppOptions> options)
    {
        _options = options.Value;
    }

    /// <summary>Возвращает статус приложения.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthDto), StatusCodes.Status200OK)]
    public ActionResult<HealthDto> Get() =>
        Ok(new HealthDto("ok", _options.ApplicationName, DateTime.UtcNow));
}

/// <summary>Ответ health-check.</summary>
/// <param name="Status">Статус приложения.</param>
/// <param name="Application">Имя приложения из конфигурации.</param>
/// <param name="Timestamp">Время проверки (UTC).</param>
public sealed record HealthDto(string Status, string Application, DateTime Timestamp);
