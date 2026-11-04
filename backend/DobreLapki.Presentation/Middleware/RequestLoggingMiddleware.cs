using System.Diagnostics;

namespace DobreLapki.Presentation.Middleware;

/// <summary>
/// Логирует начало обработки запроса до контроллера и завершение (статус + время) после него.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>Создаёт middleware.</summary>
    public RequestLoggingMiddleware(
        RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Обрабатывает запрос с логированием до и после.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation(
            "Начало обработки запроса {Method} {Path}",
            context.Request.Method,
            context.Request.Path);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Запрос завершён {StatusCode} за {ElapsedMilliseconds} мс ({Method} {Path})",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                context.Request.Method,
                context.Request.Path);
        }
    }
}
