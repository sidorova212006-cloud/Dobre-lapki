using System.ComponentModel.DataAnnotations;

namespace DobreLapki.Presentation.Contracts.Favorites;

/// <summary>Параметры выборки избранного.</summary>
public sealed class FavoriteQuery
{
    /// <summary>Номер страницы.</summary>
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    /// <summary>Размер страницы (1–100).</summary>
    [Range(1, 100)]
    public int PageSize { get; set; } = 100;
}
