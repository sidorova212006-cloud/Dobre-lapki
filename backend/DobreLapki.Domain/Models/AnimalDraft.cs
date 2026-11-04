using DobreLapki.Domain.Enums;

namespace DobreLapki.Domain.Models;

/// <summary>Нормализованные данные для создания или обновления животного.</summary>
public sealed record AnimalDraft(
    string Name,
    AnimalType Type,
    AnimalGender Gender,
    double AgeYears,
    string Breed,
    string Description,
    IReadOnlyList<string>? Photos,
    string Health,
    bool Sterilized,
    bool Vaccinated,
    DateOnly ArrivedAt);
