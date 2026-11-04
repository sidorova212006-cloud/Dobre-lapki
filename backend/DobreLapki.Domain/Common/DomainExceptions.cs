namespace DobreLapki.Domain.Common;

/// <summary>Базовое исключение домена.</summary>
public class DomainException : Exception
{
    /// <summary>Создаёт доменное исключение с сообщением для клиента.</summary>
    public DomainException(string message) : base(message)
    {
    }
}

/// <summary>Ошибка валидации входных данных (HTTP 400).</summary>
public sealed class DomainValidationException : DomainException
{
    /// <summary>Создаёт ошибку валидации.</summary>
    public DomainValidationException(string message) : base(message)
    {
    }
}

/// <summary>Запрошенный объект не найден (HTTP 404).</summary>
public sealed class NotFoundException : DomainException
{
    /// <summary>Создаёт ошибку «не найдено».</summary>
    public NotFoundException(string message) : base(message)
    {
    }
}

/// <summary>Конфликт состояния или дубликат (HTTP 409).</summary>
public sealed class ConflictException : DomainException
{
    /// <summary>Создаёт ошибку конфликта.</summary>
    public ConflictException(string message) : base(message)
    {
    }
}
