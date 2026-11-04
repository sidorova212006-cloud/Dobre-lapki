namespace DobreLapki.Presentation.Contracts.Common;

/// <summary>Единый формат ошибки API.</summary>
/// <param name="Code">Машиночитаемый код ошибки.</param>
/// <param name="Message">Понятное пользователю сообщение.</param>
/// <param name="TraceId">Идентификатор запроса для поддержки.</param>
public sealed record ErrorResponse(string Code, string Message, string TraceId);
