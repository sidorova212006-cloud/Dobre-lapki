using DobreLapki.Domain.Enums;

namespace DobreLapki.Domain.Filters;

/// <summary>Фильтр и сортировка каталога животных.</summary>
public sealed record AnimalFilter(
    IReadOnlyList<AnimalType> Types,
    IReadOnlyList<AnimalGender> Genders,
    IReadOnlyList<AgeBucket> Ages,
    string? Search,
    AnimalSort Sort)
{
    /// <summary>Пустой фильтр: все животные, сначала новые.</summary>
    public static AnimalFilter Empty { get; } =
        new(Array.Empty<AnimalType>(), Array.Empty<AnimalGender>(), Array.Empty<AgeBucket>(), null, AnimalSort.Newest);
}
