namespace DobreLapki.Domain.Storage;

/// <summary>
/// Абстракция файлового хранилища. Пока используется mock-реализация в Infrastructure,
/// позже её место займёт работа с диском или объектным хранилищем.
/// </summary>
public interface IFileStorage
{
    /// <summary>Сохраняет файл и возвращает публичную ссылку.</summary>
    Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken);

    /// <summary>Удаляет файл по публичной ссылке.</summary>
    Task<bool> DeleteAsync(string url, CancellationToken cancellationToken);

    /// <summary>Открывает файл на чтение по публичной ссылке.</summary>
    Task<Stream?> OpenReadAsync(string url, CancellationToken cancellationToken);
}
