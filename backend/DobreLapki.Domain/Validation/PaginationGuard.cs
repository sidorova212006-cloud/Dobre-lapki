using DobreLapki.Domain.Common;

namespace DobreLapki.Domain.Validation;

/// <summary>Проверка параметров пагинации на уровне домена.</summary>
public static class PaginationGuard
{
    /// <summary>Максимально допустимый размер страницы.</summary>
    public const int MaxPageSize = 100;

    /// <summary>Проверяет номер страницы и размер страницы.</summary>
    public static void Validate(int page, int pageSize)
    {
        if (page < 1)
        {
            throw new DomainValidationException("Номер страницы должен быть не меньше 1.");
        }

        if (pageSize < 1 || pageSize > MaxPageSize)
        {
            throw new DomainValidationException(
                $"Размер страницы должен быть от 1 до {MaxPageSize}.");
        }
    }

    /// <summary>Количество элементов, которые нужно пропустить.</summary>
    public static int Skip(int page, int pageSize) => (page - 1) * pageSize;
}
