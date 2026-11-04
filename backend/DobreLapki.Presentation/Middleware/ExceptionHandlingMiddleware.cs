using DobreLapki.Domain.Common;
using DobreLapki.Presentation.Contracts.Common;

namespace DobreLapki.Presentation.Middleware;

/// <summary>
/// Глобальная обработка исключений: наружу отдаётся единый формат {code, message, traceId},
/// стек-трейс остаётся только в логах.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>Создаёт middleware.</summary>
    public ExceptionHandlingMiddleware(
        RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Обрабатывает запрос и перехватывает необработанные исключения.</summary>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            if (context.Response.HasStarted)
            {
                _logger.LogError(
                    exception,
                    "Исключение после начала ответа. TraceId={TraceId}",
                    context.TraceIdentifier);
                throw;
            }

            var (status, code, message) = Map(exception);

            if (status >= StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(
                    exception,
                    "Необработанное исключение. TraceId={TraceId}",
                    context.TraceIdentifier);
            }
            else
            {
                _logger.LogWarning(
                    "Ошибка обработки запроса {Code}. TraceId={TraceId}",
                    code,
                    context.TraceIdentifier);
            }

            context.Response.Clear();
            context.Response.StatusCode = status;
            context.Response.ContentType = "application/json; charset=utf-8";
            await context.Response.WriteAsJsonAsync(
                new ErrorResponse(code, message, context.TraceIdentifier));
        }
    }

    private static (int Status, string Code, string Message) Map(Exception exception) => exception switch
    {
        DomainValidationException => (
            StatusCodes.Status400BadRequest, "validation_error", exception.Message),
        NotFoundException => (
            StatusCodes.Status404NotFound, "not_found", exception.Message),
        ConflictException => (
            StatusCodes.Status409Conflict, "conflict", exception.Message),
        _ => (
            StatusCodes.Status500InternalServerError,
            "internal_error",
            "Внутренняя ошибка сервера. Попробуйте позже.")
    };
}
