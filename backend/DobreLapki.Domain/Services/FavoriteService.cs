using DobreLapki.Domain.Common;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Domain.Services;

/// <summary>Реализация доменного сервиса избранного.</summary>
public sealed class FavoriteService : IFavoriteService
{
    private readonly IFavoritesRepository _favorites;
    private readonly IAnimalRepository _animals;

    /// <summary>Создаёт сервис поверх репозиториев избранного и животных.</summary>
    public FavoriteService(IFavoritesRepository favorites, IAnimalRepository animals)
    {
        _favorites = favorites;
        _animals = animals;
    }

    /// <inheritdoc />
    public Task<PagedResult<int>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        PaginationGuard.Validate(page, pageSize);
        return _favorites.GetPagedAsync(page, pageSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddAsync(int animalId, CancellationToken cancellationToken)
    {
        if (animalId <= 0)
        {
            throw new DomainValidationException("Идентификатор животного должен быть положительным.");
        }

        _ = await _animals.GetByIdAsync(animalId, cancellationToken)
            ?? throw new NotFoundException($"Животное с идентификатором {animalId} не найдено.");

        if (!await _favorites.AddAsync(animalId, cancellationToken))
        {
            throw new ConflictException("Животное уже находится в избранном.");
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(int animalId, CancellationToken cancellationToken)
    {
        if (animalId <= 0)
        {
            throw new DomainValidationException("Идентификатор животного должен быть положительным.");
        }

        if (!await _favorites.RemoveAsync(animalId, cancellationToken))
        {
            throw new NotFoundException("Животное не найдено в избранном.");
        }
    }
}
