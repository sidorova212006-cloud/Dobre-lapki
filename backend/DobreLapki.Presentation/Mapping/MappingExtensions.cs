using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Models;
using DobreLapki.Presentation.Contracts.Animals;
using DobreLapki.Presentation.Contracts.Applications;
using DobreLapki.Presentation.Contracts.Users;

namespace DobreLapki.Presentation.Mapping;

/// <summary>Преобразование между DTO, доменными моделями и сущностями.</summary>
public static class MappingExtensions
{
    /// <summary>Преобразует животное в DTO.</summary>
    public static AnimalDto ToDto(this Animal animal) => new()
    {
        Id = animal.Id,
        Name = animal.Name,
        Type = animal.Type,
        Gender = animal.Gender,
        AgeYears = animal.AgeYears,
        Breed = animal.Breed,
        Description = animal.Description,
        Photos = animal.Photos.ToArray(),
        Health = animal.Health,
        Sterilized = animal.Sterilized,
        Vaccinated = animal.Vaccinated,
        ArrivedAt = animal.ArrivedAt
    };

    /// <summary>Преобразует заявку в DTO вместе с животным.</summary>
    public static ApplicationDto ToDto(this AdoptionApplication application) => new()
    {
        Id = application.Id,
        Animal = application.Animal.ToDto(),
        Status = application.Status,
        FullName = application.FullName,
        Email = application.Email,
        Phone = application.Phone,
        About = application.About,
        CreatedAt = application.CreatedAt
    };

    /// <summary>Преобразует пользователя в DTO без секретов.</summary>
    public static UserDto ToDto(this User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        Email = user.Email,
        Phone = user.Phone,
        Avatar = user.Avatar
    };

    /// <summary>Преобразует запрос создания животного в доменный черновик.</summary>
    public static AnimalDraft ToDraft(this CreateAnimalRequest request) => new(
        request.Name,
        request.Type,
        request.Gender,
        request.AgeYears,
        request.Breed,
        request.Description,
        request.Photos,
        request.Health,
        request.Sterilized,
        request.Vaccinated,
        request.ArrivedAt);

    /// <summary>Преобразует запрос создания заявки в доменный черновик.</summary>
    public static ApplicationDraft ToDraft(this CreateApplicationRequest request) => new(
        request.AnimalId,
        request.FullName,
        request.Email,
        request.Phone,
        request.About);

    /// <summary>Преобразует запрос регистрации в доменный черновик.</summary>
    public static UserRegistration ToDraft(this RegisterUserRequest request) => new(
        request.Name,
        request.Email,
        request.Phone,
        request.Password);

    /// <summary>Преобразует запрос входа в доменный черновик.</summary>
    public static UserLogin ToDraft(this LoginRequest request) => new(
        request.Email,
        request.Password);

    /// <summary>Преобразует запрос обновления профиля в доменный черновик.</summary>
    public static UserUpdateDraft ToDraft(this UpdateUserRequest request) => new(
        request.Name,
        request.Email,
        request.Phone);
}
