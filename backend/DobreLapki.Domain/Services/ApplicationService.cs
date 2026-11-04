using DobreLapki.Domain.Common;
using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Models;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Validation;

namespace DobreLapki.Domain.Services;

/// <summary>Реализация доменного сервиса заявок.</summary>
public sealed class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _repository;
    private readonly IAnimalRepository _animals;

    /// <summary>Создаёт сервис поверх репозиториев заявок и животных.</summary>
    public ApplicationService(IApplicationRepository repository, IAnimalRepository animals)
    {
        _repository = repository;
        _animals = animals;
    }

    /// <inheritdoc />
    public Task<PagedResult<AdoptionApplication>> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken)
    {
        PaginationGuard.Validate(page, pageSize);
        return _repository.GetPagedAsync(page, pageSize, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AdoptionApplication> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("Идентификатор заявки должен быть положительным.");
        }

        return await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Заявка с идентификатором {id} не найдена.");
    }

    /// <inheritdoc />
    public async Task<AdoptionApplication> CreateAsync(
        ApplicationDraft draft, CancellationToken cancellationToken)
    {
        var normalized = ApplicationRules.Normalize(draft);

        var animal = await _animals.GetByIdAsync(normalized.AnimalId, cancellationToken)
            ?? throw new NotFoundException(
                $"Животное с идентификатором {normalized.AnimalId} не найдено.");

        if (await _repository.ExistsPendingAsync(
                normalized.AnimalId, normalized.Email, cancellationToken))
        {
            throw new ConflictException(
                "Заявка на это животное от этого email уже находится на рассмотрении.");
        }

        var application = new AdoptionApplication
        {
            Animal = animal,
            Status = ApplicationStatus.Pending,
            FullName = normalized.FullName,
            Email = normalized.Email,
            Phone = normalized.Phone,
            About = normalized.About,
            CreatedAt = DateTime.UtcNow
        };

        return await _repository.CreateAsync(application, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<AdoptionApplication> UpdateAsync(
        int id, ApplicationUpdateDraft draft, CancellationToken cancellationToken)
    {
        var existing = await GetByIdAsync(id, cancellationToken);

        if (!Enum.IsDefined(draft.Status))
        {
            throw new DomainValidationException("Недопустимый статус заявки.");
        }

        if (existing.Status != ApplicationStatus.Pending && existing.Status != draft.Status)
        {
            throw new ConflictException("Решение по заявке уже принято и не может быть изменено.");
        }

        existing.Status = draft.Status;
        return await _repository.UpdateAsync(existing, cancellationToken)
            ?? throw new NotFoundException($"Заявка с идентификатором {id} не найдена.");
    }

    /// <inheritdoc />
    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            throw new DomainValidationException("Идентификатор заявки должен быть положительным.");
        }

        if (!await _repository.DeleteAsync(id, cancellationToken))
        {
            throw new NotFoundException($"Заявка с идентификатором {id} не найдена.");
        }
    }
}
