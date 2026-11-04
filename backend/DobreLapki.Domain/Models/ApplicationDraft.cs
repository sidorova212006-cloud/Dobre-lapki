using DobreLapki.Domain.Enums;

namespace DobreLapki.Domain.Models;

/// <summary>Данные для создания заявки на усыновление.</summary>
public sealed record ApplicationDraft(
    int AnimalId,
    string FullName,
    string Email,
    string Phone,
    string About);

/// <summary>Данные для изменения статуса заявки.</summary>
public sealed record ApplicationUpdateDraft(ApplicationStatus Status);
