using System.Net;
using System.Net.Http.Json;
using DobreLapki.Presentation.Contracts.Common;
using DobreLapki.Presentation.Contracts.Users;

namespace DobreLapki.Tests.Integration;

public sealed class UsersApiTests
{
    [Fact]
    public async Task Register_ReturnsCreatedWithoutSecrets()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/users/register",
            new RegisterUserRequest
            {
                Name = "Иван",
                Email = "ivan@example.com",
                Phone = "+7 900 111-22-33",
                Password = "secret123"
            },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var raw = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("password", raw, StringComparison.OrdinalIgnoreCase);

        var created = await response.Content.ReadFromJsonAsync<UserDto>(TestJson.Options);
        Assert.NotNull(created);
        Assert.Equal("ivan@example.com", created!.Email);
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsConflict()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterUserRequest
        {
            Name = "Иван",
            Email = "duplicate@example.com",
            Password = "secret123"
        };

        var first = await client.PostAsJsonAsync("/api/v1/users/register", request, TestJson.Options);
        var second = await client.PostAsJsonAsync("/api/v1/users/register", request, TestJson.Options);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Register_ShortPassword_ReturnsValidationError()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/users/register",
            new RegisterUserRequest
            {
                Name = "Иван",
                Email = "short@example.com",
                Password = "123"
            },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("validation_error", error!.Code);
    }

    [Fact]
    public async Task Login_WithDemoCredentials_ReturnsUser()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/users/login",
            new LoginRequest { Email = "user@example.com", Password = "password123" },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = await response.Content.ReadFromJsonAsync<UserDto>(TestJson.Options);
        Assert.NotNull(user);
        Assert.Equal("user@example.com", user!.Email);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsValidationError()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/v1/users/login",
            new LoginRequest { Email = "user@example.com", Password = "wrong-password" },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("validation_error", error!.Code);
    }

    [Fact]
    public async Task GetById_Unknown_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/users/999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ReturnsUpdatedProfile()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/v1/users/1",
            new UpdateUserRequest
            {
                Name = "Анна Смирнова",
                Email = "user@example.com",
                Phone = "+7 900 123-45-67"
            },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updated = await response.Content.ReadFromJsonAsync<UserDto>(TestJson.Options);
        Assert.Equal("Анна Смирнова", updated!.Name);
        Assert.Equal("+7 900 123-45-67", updated.Phone);
    }

    [Fact]
    public async Task Update_Unknown_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PutAsJsonAsync(
            "/api/v1/users/999",
            new UpdateUserRequest { Name = "Никто", Email = "nobody@example.com" },
            TestJson.Options);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
