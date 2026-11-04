using System.Net;
using System.Net.Http.Json;
using DobreLapki.Domain.Common;
using DobreLapki.Domain.Enums;
using DobreLapki.Presentation.Contracts.Applications;
using DobreLapki.Presentation.Contracts.Common;

namespace DobreLapki.Tests.Integration;

public sealed class ApplicationsApiTests
{
    private static CreateApplicationRequest CreateRequest(int animalId, string email) => new()
    {
        AnimalId = animalId,
        FullName = "Иван Петров",
        Email = email,
        Phone = "+7 900 111-22-33",
        About = "Люблю животных, есть опыт содержания собак и кошек."
    };

    [Fact]
    public async Task GetAll_ReturnsSeededApplications()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/applications?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<ApplicationDto>>(TestJson.Options);
        Assert.NotNull(page);
        Assert.Equal(2, page!.TotalCount);
    }

    [Fact]
    public async Task Create_ReturnsCreatedWithAnimal()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/applications", CreateRequest(2, "new-applicant@example.com"), TestJson.Options);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<ApplicationDto>(TestJson.Options);
        Assert.NotNull(created);
        Assert.Equal(ApplicationStatus.Pending, created!.Status);
        Assert.Equal(2, created.Animal.Id);
    }

    [Fact]
    public async Task Create_UnknownAnimal_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/applications", CreateRequest(999, "no-animal@example.com"), TestJson.Options);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("not_found", error!.Code);
    }

    [Fact]
    public async Task Create_InvalidPayload_ReturnsValidationError()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = CreateRequest(2, "invalid@example.com");
        request.About = "мало";

        var response = await client.PostAsJsonAsync("/api/v1/applications", request, TestJson.Options);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("validation_error", error!.Code);
    }

    [Fact]
    public async Task Create_DuplicatePending_ReturnsConflict()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = CreateRequest(2, "duplicate@example.com");
        var first = await client.PostAsJsonAsync("/api/v1/applications", request, TestJson.Options);
        var second = await client.PostAsJsonAsync("/api/v1/applications", request, TestJson.Options);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);

        var error = await second.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("conflict", error!.Code);
    }

    [Fact]
    public async Task UpdateStatus_ApprovedThenRejected_ReturnsConflict()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var approve = await client.PutAsJsonAsync(
            "/api/v1/applications/2",
            new UpdateApplicationRequest { Status = ApplicationStatus.Approved },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.OK, approve.StatusCode);
        var approved = await approve.Content.ReadFromJsonAsync<ApplicationDto>(TestJson.Options);
        Assert.Equal(ApplicationStatus.Approved, approved!.Status);

        var reject = await client.PutAsJsonAsync(
            "/api/v1/applications/2",
            new UpdateApplicationRequest { Status = ApplicationStatus.Rejected },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.Conflict, reject.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesApplication()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/v1/applications/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var fetched = await client.GetAsync("/api/v1/applications/1");
        Assert.Equal(HttpStatusCode.NotFound, fetched.StatusCode);
    }
}
