using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Filters;
using DobreLapki.Domain.Models;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Domain.Services;

/// <summary>Реализация доменного сервиса животных.</summary>
public sealed class AnimalService : IAnimalService
{
    private readonly IAnimalRepository _repository;

    /// <summary>Создаёт сервис поверх репозитория животных.</summary>
    public AnimalService(IAnimalRepository repository)
    {
        _repository = repository;
    }

    /// <inheritdoc />
    public Task<PagedResult<Animal>> GetPagedAsync(
        AnimalFilter filter, int page, int pageSize, CancellationToken cancellationToken)
    {
        PaginationGuard.Validate(page, pageSize);
        return _repository.GetPagedAsync(filter, page, pageSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Animal> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("Идентификатор животного должен быть положительным.");
        }

        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Животное с идентификатором {id} не найдено.");
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Animal>> GetPopularAsync(int count, CancellationToken cancellationToken)
    {
        var safeCount = Math.Clamp(count, 1, 20);
        return _repository.GetPopularAsync(safeCount, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Animal> CreateAsync(AnimalDraft draft, CancellationToken cancellationToken)
    {
        var normalized = AnimalRules.Normalize(draft);

        if (await _repository.ExistsByNameAndTypeAsync(
                normalized.Name, normalized.Type, excludeId: null, cancellationToken))
        {
            throw new ConflictException(
                "Животное с такой кличкой и видом уже есть в каталоге.");
        }

        var animal = Map(normalized);
        return await _repository.CreateAsync(animal, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<Animal> UpdateAsync(int id, AnimalDraft draft, CancellationToken cancellationToken)
    {
        _ = await GetByIdAsync(id, cancellationToken);
        var normalized = AnimalRules.Normalize(draft);

        if (await _repository.ExistsByNameAndTypeAsync(
                normalized.Name, normalized.Type, excludeId: id, cancellationToken))
        {
            throw new ConflictException(
                "Животное с такой кличкой и видом уже есть в каталоге.");
        }

        var animal = Map(normalized);
        animal.Id = id;

        return await _repository.UpdateAsync(animal, cancellationToken)
            ?? throw new NotFoundException($"Животное с идентификатором {id} не найдено.");
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("Идентификатор животного должен быть положительным.");
        }

        if (!await _repository.DeleteAsync(id, cancellationToken))
        {
            throw new NotFoundException($"Животное с идентификатором {id} не найдено.");
        }
    }

    private static Animal Map(AnimalDraft draft) => new()
    {
        Name = draft.Name,
        Type = draft.Type,
        Gender = draft.Gender,
        AgeYears = draft.AgeYears,
        Breed = draft.Breed,
        Description = draft.Description,
        Photos = draft.Photos?.ToList() ?? new List<string>(),
        Health = draft.Health,
        Sterilized = draft.Sterilized,
        Vaccinated = draft.Vaccinated,
        ArrivedAt = draft.ArrivedAt
    };
}
