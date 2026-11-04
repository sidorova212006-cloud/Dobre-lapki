using System.ComponentModel.DataAnnotations;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;

namespace DobreLapki.Presentation.Contracts.Animals;

/// <summary>Животное, отдаваемое клиенту.</summary>
public sealed class AnimalDto
{
    /// <summary>Идентификатор.</summary>
    public int Id { get; init; }

    /// <summary>Кличка.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Вид животного (cat, dog, other).</summary>
    public AnimalType Type { get; init; }

    /// <summary>Пол (male, female).</summary>
    public AnimalGender Gender { get; init; }

    /// <summary>Возраст в годах.</summary>
    public double AgeYears { get; init; }

    /// <summary>Порода.</summary>
    public string Breed { get; init; } = string.Empty;

    /// <summary>Описание.</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Фотографии.</summary>
    public IReadOnlyList<string> Photos { get; init; } = Array.Empty<string>();

    /// <summary>Состояние здоровья.</summary>
    public string Health { get; init; } = string.Empty;

    /// <summary>Стерилизовано ли животное.</summary>
    public bool Sterilized { get; init; }

    /// <summary>Сделаны ли прививки.</summary>
    public bool Vaccinated { get; init; }

    /// <summary>Дата поступления в приют (yyyy-MM-dd).</summary>
    public DateOnly ArrivedAt { get; init; }
}

/// <summary>Запрос на создание животного.</summary>
public class CreateAnimalRequest
{
    /// <summary>Кличка (1–100 символов).</summary>
    [Required(ErrorMessage = "Кличка обязательна.")]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Вид животного.</summary>
    [EnumDataType(typeof(AnimalType), ErrorMessage = "Недопустимый вид животного.")]
    public AnimalType Type { get; set; }

    /// <summary>Пол животного.</summary>
    [EnumDataType(typeof(AnimalGender), ErrorMessage = "Недопустимый пол животного.")]
    public AnimalGender Gender { get; set; }

    /// <summary>Возраст в годах (0–50).</summary>
    [Range(0, 50, ErrorMessage = "Возраст должен быть от 0 до 50 лет.")]
    public double AgeYears { get; set; }

    /// <summary>Порода (1–100 символов).</summary>
    [Required(ErrorMessage = "Порода обязательна.")]
    [StringLength(100, MinimumLength = 1)]
    public string Breed { get; set; } = string.Empty;

    /// <summary>Описание (10–2000 символов).</summary>
    [Required(ErrorMessage = "Описание обязательно.")]
    [StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    /// <summary>Ссылки на фотографии (не больше 10).</summary>
    [MaxLength(10, ErrorMessage = "Не больше 10 фотографий.")]
    public List<string>? Photos { get; set; }

    /// <summary>Состояние здоровья (1–200 символов).</summary>
    [Required(ErrorMessage = "Состояние здоровья обязательно.")]
    [StringLength(200, MinimumLength = 1)]
    public string Health { get; set; } = string.Empty;

    /// <summary>Стерилизовано ли животное.</summary>
    public bool Sterilized { get; set; }

    /// <summary>Сделаны ли прививки.</summary>
    public bool Vaccinated { get; set; }

    /// <summary>Дата поступления (не в будущем).</summary>
    [Required]
    public DateOnly ArrivedAt { get; set; }
}

/// <summary>Запрос на полное обновление животного.</summary>
public sealed class UpdateAnimalRequest : CreateAnimalRequest
{
}

/// <summary>Параметры выборки каталога животных.</summary>
public sealed class AnimalQuery
{
    /// <summary>Номер страницы, начиная с 1.</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Номер страницы должен быть не меньше 1.")]
    public int Page { get; set; } = 1;

    /// <summary>Размер страницы (1–100).</summary>
    [Range(1, 100, ErrorMessage = "Размер страницы должен быть от 1 до 100.")]
    public int PageSize { get; set; } = 20;

    /// <summary>Виды через запятую: cat,dog,other.</summary>
    [StringLength(50)]
    public string? Types { get; set; }

    /// <summary>Пол через запятую: male,female.</summary>
    [StringLength(30)]
    public string? Genders { get; set; }

    /// <summary>Возрастные диапазоны через запятую: under1,from1to3,over3.</summary>
    [StringLength(50)]
    public string? Ages { get; set; }

    /// <summary>Поиск по кличке или породе.</summary>
    [StringLength(100)]
    public string? Search { get; set; }

    /// <summary>Сортировка: newest, oldest, name.</summary>
    [StringLength(20)]
    public string? Sort { get; set; }
}

/// <summary>Ответ на запрос популярных животных.</summary>
public sealed record PopularAnimalsQuery
{
    /// <summary>Сколько животных вернуть (1–20).</summary>
    [Range(1, 20)]
    public int Count { get; init; } = 5;
}
