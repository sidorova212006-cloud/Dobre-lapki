using DobreLapki.Domain.Entities;
using DobreLapki.Domain.Enums;
using DobreLapki.Domain.Filters;
using DobreLapki.Infrastructure.Repositories;
using DobreLapki.Infrastructure.Security;
using DobreLapki.Infrastructure.Storage;

namespace DobreLapki.Tests.Infrastructure;

public sealed class InMemoryRepositoriesTests
{
    private static Animal NewAnimal(
        int id,
        string name,
        AnimalType type = AnimalType.Cat,
        AnimalGender gender = AnimalGender.Female,
        double age = 2,
        DateOnly? arrivedAt = null)
    {
        return new Animal
        {
            Id = id,
            Name = name,
            Type = type,
            Gender = gender,
            AgeYears = age,
            Breed = "Метис",
            Description = "Описание животного для теста.",
            Photos = new List<string>(),
            Health = "в норме",
            ArrivedAt = arrivedAt ?? new DateOnly(2026, 1, 1)
        };
    }

    [Fact]
    public async Task AnimalRepository_CrudCycle()
    {
        var repository = InMemoryAnimalRepository.CreateWith(new[] { NewAnimal(1, "Мурка") });

        var created = await repository.CreateAsync(NewAnimal(0, "Рекс", AnimalType.Dog), default);
        Assert.Equal(2, created.Id);

        var fetched = await repository.GetByIdAsync(created.Id, default);
        Assert.NotNull(fetched);

        fetched!.Name = "Рекс-младший";
        var updated = await repository.UpdateAsync(fetched, default);
        Assert.NotNull(updated);
        Assert.Equal("Рекс-младший", updated!.Name);

        Assert.True(await repository.DeleteAsync(created.Id, default));
        Assert.Null(await repository.GetByIdAsync(created.Id, default));
        Assert.False(await repository.DeleteAsync(created.Id, default));
    }

    [Fact]
    public async Task AnimalRepository_FiltersByTypeAndSearch()
    {
        var repository = InMemoryAnimalRepository.CreateWith(new[]
        {
            NewAnimal(1, "Мурка", AnimalType.Cat),
            NewAnimal(2, "Рекс", AnimalType.Dog),
            NewAnimal(3, "Луна", AnimalType.Cat)
        });

        var filter = new AnimalFilter(
            new[] { AnimalType.Cat },
            Array.Empty<AnimalGender>(),
            Array.Empty<AgeBucket>(),
            "Мур",
            AnimalSort.Newest);

        var result = await repository.GetPagedAsync(filter, 1, 10, default);

        Assert.Single(result.Items);
        Assert.Equal("Мурка", result.Items[0].Name);
        Assert.Equal(1, result.TotalCount);
    }

    [Fact]
    public async Task AnimalRepository_SortsByNameAndPages()
    {
        var repository = InMemoryAnimalRepository.CreateWith(new[]
        {
            NewAnimal(1, "Яша"),
            NewAnimal(2, "Ася"),
            NewAnimal(3, "Барсик")
        });

        var result = await repository.GetPagedAsync(
            AnimalFilter.Empty with { Sort = AnimalSort.Name }, 1, 2, default);

        Assert.Equal(2, result.Items.Count);
        Assert.Equal("Ася", result.Items[0].Name);
        Assert.Equal("Барсик", result.Items[1].Name);
        Assert.Equal(3, result.TotalCount);
    }

    [Fact]
    public async Task AnimalRepository_ExistsByNameAndType_IgnoresCase()
    {
        var repository = InMemoryAnimalRepository.CreateWith(new[] { NewAnimal(1, "Мурка") });

        Assert.True(await repository.ExistsByNameAndTypeAsync(
            "мурка", AnimalType.Cat, null, default));
        Assert.False(await repository.ExistsByNameAndTypeAsync(
            "мурка", AnimalType.Dog, null, default));
        Assert.False(await repository.ExistsByNameAndTypeAsync(
            "мурка", AnimalType.Cat, 1, default));
    }

    [Fact]
    public async Task ApplicationRepository_CrudCycle()
    {
        var animal = NewAnimal(1, "Мурка");
        var repository = InMemoryApplicationRepository.CreateWith(Array.Empty<AdoptionApplication>());

        var application = new AdoptionApplication
        {
            Animal = animal,
            Status = ApplicationStatus.Pending,
            FullName = "Иван Петров",
            Email = "ivan@example.com",
            Phone = "+7 900 111-22-33",
            About = "Опыт содержания животных",
            CreatedAt = DateTime.UtcNow
        };

        var created = await repository.CreateAsync(application, default);
        Assert.Equal(1, created.Id);
        Assert.True(await repository.ExistsPendingAsync(1, "IVAN@example.com", default));

        var fetched = await repository.GetByIdAsync(created.Id, default);
        Assert.NotNull(fetched);

        fetched!.Status = ApplicationStatus.Approved;
        var updated = await repository.UpdateAsync(fetched, default);
        Assert.Equal(ApplicationStatus.Approved, updated!.Status);
        Assert.False(await repository.ExistsPendingAsync(1, "ivan@example.com", default));

        var page = await repository.GetPagedAsync(1, 10, default);
        Assert.Single(page.Items);
        Assert.Equal(1, page.TotalCount);

        Assert.True(await repository.DeleteAsync(created.Id, default));
        Assert.Null(await repository.GetByIdAsync(created.Id, default));
    }

    [Fact]
    public async Task UserRepository_CrudAndEmailLookup()
    {
        var repository = InMemoryUserRepository.CreateWith(new[]
        {
            new User { Id = 1, Name = "Анна", Email = "anna@example.com" }
        });

        var byEmail = await repository.GetByEmailAsync("ANNA@example.com", default);
        Assert.NotNull(byEmail);
        Assert.Equal("Анна", byEmail!.Name);

        var created = await repository.CreateAsync(
            new User { Name = "Иван", Email = "ivan@example.com" }, default);
        Assert.Equal(2, created.Id);

        created.Name = "Иван Петров";
        var updated = await repository.UpdateAsync(created, default);
        Assert.Equal("Иван Петров", updated!.Name);

        var page = await repository.GetPagedAsync(1, 10, default);
        Assert.Equal(2, page.TotalCount);

        Assert.True(await repository.DeleteAsync(created.Id, default));
        Assert.Null(await repository.GetByIdAsync(created.Id, default));
    }

    [Fact]
    public async Task FavoritesRepository_AddRemoveAndPage()
    {
        var repository = InMemoryFavoritesRepository.CreateWith(new[] { 1 });

        Assert.True(await repository.ExistsAsync(1, default));
        Assert.True(await repository.AddAsync(2, default));
        Assert.False(await repository.AddAsync(2, default));

        var page = await repository.GetPagedAsync(1, 10, default);
        Assert.Equal(2, page.TotalCount);
        Assert.Equal(new[] { 1, 2 }, page.Items);

        Assert.True(await repository.RemoveAsync(1, default));
        Assert.False(await repository.RemoveAsync(1, default));
        Assert.False(await repository.ExistsAsync(1, default));
    }

    [Fact]
    public async Task MockFileStorage_SaveReadDelete()
    {
        var storage = new MockFileStorage();
        using var content = new MemoryStream(new byte[] { 1, 2, 3 });

        var url = await storage.SaveAsync(content, "photo.jpg", "image/jpeg", default);
        Assert.StartsWith("/api/v1/files/", url);

        await using var read = await storage.OpenReadAsync(url, default);
        Assert.NotNull(read);
        using var buffer = new MemoryStream();
        await read!.CopyToAsync(buffer);
        Assert.Equal(new byte[] { 1, 2, 3 }, buffer.ToArray());

        Assert.True(await storage.DeleteAsync(url, default));
        Assert.Null(await storage.OpenReadAsync(url, default));
    }

    [Fact]
    public void PasswordHasher_HashAndVerify()
    {
        var hasher = new PasswordHasher();
        var (hash, salt) = hasher.HashPassword("secret123");

        Assert.True(hasher.Verify("secret123", hash, salt));
        Assert.False(hasher.Verify("wrong-password", hash, salt));
        Assert.False(hasher.Verify("secret123", "not-base64", "not-base64"));
    }
}
