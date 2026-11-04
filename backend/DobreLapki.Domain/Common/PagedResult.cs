namespace DobreLapki.Domain.Common;

/// <summary>
/// Постраничный результат запроса списка.
/// </summary>
/// <typeparam name="T">Тип элемента страницы.</typeparam>
public sealed class PagedResult<T>
{
    /// <summary>
    /// Создаёт постраничный результат.
    /// </summary>
    public PagedResult(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    /// <summary>Элементы текущей страницы.</summary>
    public IReadOnlyList<T> Items { get; }

    /// <summary>Номер страницы (начиная с 1).</summary>
    public int Page { get; }

    /// <summary>Размер страницы.</summary>
    public int PageSize { get; }

    /// <summary>Общее количество элементов во всех страницах.</summary>
    public int TotalCount { get; }

    /// <summary>Общее количество страниц.</summary>
    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);

    /// <summary>Пустая страница.</summary>
    public static PagedResult<T> Empty(int page, int pageSize) =>
        new(Array.Empty<T>(), page, pageSize, 0);
}
