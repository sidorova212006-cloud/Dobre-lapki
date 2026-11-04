using DobreLapki.Domain.Repositories;
using DobreLapki.Infrastructure.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DobreLapki.Tests.Integration;

/// <summary>
/// Фабрика приложения для интеграционных тестов: каждый экземпляр получает
/// собственные in-memory-репозитории с демонстрационными данными.
/// </summary>
public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IAnimalRepository>();
            services.RemoveAll<IApplicationRepository>();
            services.RemoveAll<IUserRepository>();
            services.RemoveAll<IFavoritesRepository>();

            services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
            services.AddSingleton<IApplicationRepository, InMemoryApplicationRepository>();
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
            services.AddSingleton<IFavoritesRepository, InMemoryFavoritesRepository>();
        });
    }
}
