using DobreLapki.Domain.Enums;

namespace DobreLapki.Domain.Entities;

/// <summary>Заявка на усыновление животного.</summary>
public sealed class AdoptionApplication
{
    /// <summary>Идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Животное, на которое подана заявка.</summary>
    public required Animal Animal { get; set; }

    /// <summary>Статус заявки.</summary>
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;

    /// <summary>Имя и фамилия заявителя.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email заявителя.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Телефон заявителя.</summary>
    public string Phone { get; set; } = string.Empty;

    /// <summary>Рассказ о себе и опыте содержания животных.</summary>
    public string About { get; set; } = string.Empty;

    /// <summary>Дата создания заявки (UTC).</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Идентификатор животного.</summary>
    public int AnimalId => Animal.Id;
}
