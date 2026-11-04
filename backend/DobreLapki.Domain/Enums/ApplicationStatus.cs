namespace DobreLapki.Domain.Enums;

/// <summary>Статус заявки на усыновление.</summary>
public enum ApplicationStatus
{
    /// <summary>Заявка на рассмотрении.</summary>
    Pending,

    /// <summary>Заявка одобрена.</summary>
    Approved,

    /// <summary>Заявка отклонена.</summary>
    Rejected
}
