using System.Net;
using System.Net.Http.Json;
using DobreLapki.Domain.Common;
using DobreLapki.Presentation.Contracts.Common;

namespace DobreLapki.Tests.Integration;

public sealed class FavoritesApiTests
{
    [Fact]
    public async Task GetAll_InitiallyEmpty()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/favorites");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResult<int>>(TestJson.Options);
        Assert.NotNull(page);
        Assert.Equal(0, page!.TotalCount);
        Assert.Empty(page.Items);
    }

    [Fact]
    public async Task Add_ReturnsCreatedAndAppearsInList()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/v1/favorites/1", content: null);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var list = await client.GetFromJsonAsync<PagedResult<int>>("/api/v1/favorites", TestJson.Options);
        Assert.NotNull(list);
        Assert.Equal(1, list!.TotalCount);
        Assert.Contains(1, list.Items);
    }

    [Fact]
    public async Task Add_Duplicate_ReturnsConflict()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var first = await client.PostAsync("/api/v1/favorites/1", content: null);
        var second = await client.PostAsync("/api/v1/favorites/1", content: null);

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
    }

    [Fact]
    public async Task Add_UnknownAnimal_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync("/api/v1/favorites/999", content: null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(TestJson.Options);
        Assert.Equal("not_found", error!.Code);
    }

    [Fact]
    public async Task Remove_ReturnsNoContent()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        await client.PostAsync("/api/v1/favorites/1", content: null);
        var response = await client.DeleteAsync("/api/v1/favorites/1");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var list = await client.GetFromJsonAsync<PagedResult<int>>("/api/v1/favorites", TestJson.Options);
        Assert.Equal(0, list!.TotalCount);
    }

    [Fact]
    public async Task Remove_NotFavorite_ReturnsNotFound()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.DeleteAsync("/api/v1/favorites/1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Health_ReturnsOk()
    {
        using var factory = new TestWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/v1/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var raw = await response.Content.ReadAsStringAsync();
        Assert.Contains("ok", raw, StringComparison.OrdinalIgnoreCase);
    }
}
