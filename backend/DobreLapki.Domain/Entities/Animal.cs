using DobreLapki.Domain.Enums;

namespace DobreLapki.Domain.Entities;

/// <summary>Животное приюта.</summary>
public sealed class Animal
{
    /// <summary>Идентификатор.</summary>
    public int Id { get; set; }

    /// <summary>Кличка.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Вид животного.</summary>
    public AnimalType Type { get; set; }

    /// <summary>Пол.</summary>
    public AnimalGender Gender { get; set; }

    /// <summary>Возраст в годах (дробный — для котят и щенков).</summary>
    public double AgeYears { get; set; }

    /// <summary>Порода.</summary>
    public string Breed { get; set; } = string.Empty;

    /// <summary>Описание характера и особенностей.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Ссылки на фотографии.</summary>
    public List<string> Photos { get; set; } = new();

    /// <summary>Состояние здоровья.</summary>
    public string Health { get; set; } = string.Empty;

    /// <summary>Стерилизовано ли животное.</summary>
    public bool Sterilized { get; set; }

    /// <summary>Сделаны ли прививки.</summary>
    public bool Vaccinated { get; set; }

    /// <summary>Дата поступления в приют.</summary>
    public DateOnly ArrivedAt { get; set; }

    /// <summary>Возрастной диапазон, вычисляемый по возрасту.</summary>
    public AgeBucket AgeBucket =>
        AgeYears < 1 ? AgeBucket.Under1 : AgeYears <= 3 ? AgeBucket.From1To3 : AgeBucket.Over3;
}
