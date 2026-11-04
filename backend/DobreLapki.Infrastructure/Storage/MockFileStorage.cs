using DobreLapki.Domain.Storage;

namespace DobreLapki.Infrastructure.Storage;

/// <summary>
/// Mock-хранилище файлов: держит содержимое в памяти и отдаёт фиктивные ссылки.
/// Позже заменяется реализацией, работающей с диском или объектным хранилищем.
/// </summary>
public sealed class MockFileStorage : IFileStorage
{
    private readonly object _sync = new();
    private readonly Dictionary<string, StoredFile> _files = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public async Task<string> SaveAsync(
        Stream content, string fileName, string contentType, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(content);

        using var buffer = new MemoryStream();
        await content.CopyToAsync(buffer, cancellationToken);

        var safeName = Path.GetFileName(fileName);
        var key = $"{Guid.NewGuid():N}-{safeName}";

        lock (_sync)
        {
            _files[key] = new StoredFile(buffer.ToArray(), contentType);
        }

        return $"/api/v1/files/{key}";
    }

    /// <inheritdoc />
    public Task<bool> DeleteAsync(string url, CancellationToken cancellationToken)
    {
        var key = ExtractKey(url);
        if (key is null)
        {
            return Task.FromResult(false);
        }

        lock (_sync)
        {
            return Task.FromResult(_files.Remove(key));
        }
    }

    /// <inheritdoc />
    public Task<Stream?> OpenReadAsync(string url, CancellationToken cancellationToken)
    {
        var key = ExtractKey(url);
        if (key is null)
        {
            return Task.FromResult<Stream?>(null);
        }

        lock (_sync)
        {
            if (!_files.TryGetValue(key, out var file))
            {
                return Task.FromResult<Stream?>(null);
            }

            return Task.FromResult<Stream?>(new MemoryStream(file.Content, writable: false));
        }
    }

    private static string? ExtractKey(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return null;
        }

        var index = url.LastIndexOf('/');
        return index < 0 || index == url.Length - 1 ? null : url[(index + 1)..];
    }

    private sealed record StoredFile(byte[] Content, string ContentType);
}
