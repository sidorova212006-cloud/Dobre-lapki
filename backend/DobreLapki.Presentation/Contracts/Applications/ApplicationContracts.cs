using System.ComponentModel.DataAnnotations;
using DobreLapki.Domain.Enums;

namespace DobreLapki.Presentation.Contracts.Applications;

/// <summary>Заявка на усыновление, отдаваемая клиенту.</summary>
public sealed class ApplicationDto
{
    /// <summary>Идентификатор заявки.</summary>
    public int Id { get; init; }

    /// <summary>Животное, на которое подана заявка.</summary>
    public Animals.AnimalDto Animal { get; init; } = new();

    /// <summary>Статус (pending, approved, rejected).</summary>
    public ApplicationStatus Status { get; init; }

    /// <summary>Имя и фамилия заявителя.</summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>Email заявителя.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>Телефон заявителя.</summary>
    public string Phone { get; init; } = string.Empty;

    /// <summary>Рассказ о себе.</summary>
    public string About { get; init; } = string.Empty;

    /// <summary>Дата создания (UTC).</summary>
    public DateTime CreatedAt { get; init; }
}

/// <summary>Запрос на создание заявки.</summary>
public sealed class CreateApplicationRequest
{
    /// <summary>Идентификатор животного.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Идентификатор животного должен быть положительным.")]
    public int AnimalId { get; set; }

    /// <summary>Имя и фамилия (2–100 символов).</summary>
    [Required(ErrorMessage = "Имя и фамилия обязательны.")]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>Email.</summary>
    [Required(ErrorMessage = "Email обязателен.")]
    [EmailAddress(ErrorMessage = "Введите корректный email.")]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    /// <summary>Телефон (10–30 символов).</summary>
    [Required(ErrorMessage = "Телефон обязателен.")]
    [StringLength(30, MinimumLength = 10)]
    [RegularExpression("^[+()\\d\\s-]+$", ErrorMessage = "Недопустимые символы в телефоне.")]
    public string Phone { get; set; } = string.Empty;

    /// <summary>Рассказ о себе (10–1000 символов).</summary>
    [Required(ErrorMessage = "Расскажите немного о себе.")]
    [StringLength(1000, MinimumLength = 10)]
    public string About { get; set; } = string.Empty;
}

/// <summary>Запрос на изменение статуса заявки.</summary>
public sealed class UpdateApplicationRequest
{
    /// <summary>Новый статус заявки.</summary>
    [EnumDataType(typeof(ApplicationStatus), ErrorMessage = "Недопустимый статус заявки.")]
    public ApplicationStatus Status { get; set; }
}

/// <summary>Параметры выборки заявок.</summary>
public sealed class ApplicationQuery
{
    /// <summary>Номер страницы.</summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>Размер страницы (1–100).</summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 20;
}
