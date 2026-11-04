using System.Net;
using System.Net.Http.Json;
using DobreLapki.Domain.Common;
using DobreLapki.Domain.Enums;
using DobreLapki.Presentation.Contracts.Animals;
using DobreLapki.Presentation.Contracts.Common;

namespace DobreLapki.Tests.Integration;

public sealed class AnimalsApiTests
{
    private static CreateAnimalRequest CreateRequest(string name) => new()
    {
        Name = name,
        Type = AnimalType.Cat,
        Gender = AnimalGender.Male,
        AgeYears = 2,
        Breed = "Метис",
        Description = "Очень добрый и ласковый кот, который ищет новый дом.",
        Photos = new List<string> { "https://example.com/cat.jpg" },
        Health = "в норме",
        Sterilized = true,
        Vaccinated = true,
        ArrivedAt = new DateOnly(2026, 1, 1)
    };

    [Fact]
    public async Task GetAll_ReturnsPagedAnimals()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals?page=1&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<AnimalDto>>(TestJson.Options);
        Assert.NotNull(page);
        Assert.Equal(5, page!.Items.Count);
        Assert.Equal(15, page.TotalCount);
        Assert.Equal(1, page.Page);
        Assert.Equal(5, page.PageSize);
    }

    [Fact]
    public async Task GetAll_WithFilters_ReturnsFilteredAnimals()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals?types=cat&genders=female&ages=from1to3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<AnimalDto>>(TestJson.Options);
        Assert.NotNull(page);
        Assert.Equal(2, page!.TotalCount);
        Assert.All(page.Items, animal =>
        {
            Assert.Equal(AnimalType.Cat, animal.Type);
            Assert.Equal(AnimalGender.Female, animal.Gender);
            Assert.InRange(animal.AgeYears, 1d, 3d);
        });
    }

    [Fact]
    public async Task GetAll_WithInvalidType_ReturnsValidationError()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals?types=dragon");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.NotNull(error);
        Assert.Equal("validation_error", error!.Code);
    }

    [Fact]
    public async Task GetAll_WithTooLargePageSize_ReturnsValidationError()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals?pageSize=101");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("validation_error", error!.Code);
    }

    [Fact]
    public async Task GetPopular_ReturnsRequestedCount()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals/popular?count=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = await response.Content.ReadFromJsonAsync<List<AnimalDto>>(TestJson.Options);
        Assert.NotNull(items);
        Assert.Equal(3, items!.Count);
    }

    [Fact]
    public async Task GetById_ReturnsAnimal()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var animal = await response.Content.ReadFromJsonAsync<AnimalDto>(TestJson.Options);
        Assert.NotNull(animal);
        Assert.Equal(1, animal!.Id);
        Assert.Equal("Мурка", animal.Name);
    }

    [Fact]
    public async Task GetById_Unknown_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/animals/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("not_found", error!.Code);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithLocation()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/animals", CreateRequest("Тестовый кот"), TestJson.Options);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<AnimalDto>(TestJson.Options);
        Assert.NotNull(created);
        Assert.Equal("Тестовый кот", created!.Name);

        var fetched = await client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
    }

    [Fact]
    public async Task Create_DuplicateNameAndType_ReturnsConflict()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var first = await client.PostAsJsonAsync(
            "/api/v1/animals", CreateRequest("Дубликат"), TestJson.Options);
        var second = await client.PostAsJsonAsync(
            "/api/v1/animals", CreateRequest("Дубликат"), TestJson.Options);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var error = await second.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("conflict", error!.Code);
    }

    [Fact]
    public async Task Create_InvalidPayload_ReturnsValidationError()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = CreateRequest(string.Empty);
        request.Description = "коротко";

        var response = await client.PostAsJsonAsync("/api/v1/animals", request, TestJson.Options);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("validation_error", error!.Code);
    }

    [Fact]
    public async Task Update_ReturnsUpdatedAnimal()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createdResponse = await client.PostAsJsonAsync(
            "/api/v1/animals", CreateRequest("До обновления"), TestJson.Options);
        var created = await createdResponse.Content.ReadFromJsonAsync<AnimalDto>(TestJson.Options);

        var request = CreateRequest("После обновления");
        var response = await client.PutAsJsonAsync(
            $"/api/v1/animals/{created!.Id}", request, TestJson.Options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<AnimalDto>(TestJson.Options);
        Assert.Equal("После обновления", updated!.Name);
    }

    [Fact]
    public async Task Update_Unknown_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/v1/animals/999", CreateRequest("Неизвестный"), TestJson.Options);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesAnimal()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var createdResponse = await client.PostAsJsonAsync(
            "/api/v1/animals", CreateRequest("Удаляемый"), TestJson.Options);
        var created = await createdResponse.Content.ReadFromJsonAsync<AnimalDto>(TestJson.Options);

        var response = await client.DeleteAsync($"/api/v1/animals/{created!.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await client.GetAsync($"/api/v1/animals/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, fetched.StatusCode);
    }

    [Fact]
    public async Task Delete_Unknown_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/v1/animals/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
