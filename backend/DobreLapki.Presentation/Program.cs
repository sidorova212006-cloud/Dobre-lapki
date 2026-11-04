using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using DobreLapki.Domain.Repositories;
using DobreLapki.Domain.Security;
using DobreLapki.Domain.Services;
using DobreLapki.Domain.Storage;
using DobreLapki.Infrastructure.Repositories;
using DobreLapki.Infrastructure.Security;
using DobreLapki.Infrastructure.Storage;
using DobreLapki.Presentation.Contracts.Common;
using DobreLapki.Presentation.Middleware;
using DobreLapki.Presentation.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Секреты не хранятся в git: шаблон — appsettings.Secrets.example.json.
builder.Configuration.AddJsonFile(
    "appsettings.Secrets.json", optional: true, reloadOnChange: true);

builder.Services
    .AddOptions<AppOptions>()
    .Bind(builder.Configuration.GetSection(AppOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddOptions<PaginationOptions>()
    .Bind(builder.Configuration.GetSection(PaginationOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddSingleton<IValidateOptions<PaginationOptions>, PaginationOptionsValidator>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var messages = context.ModelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct()
            .ToArray();

        var message = messages.Length > 0
            ? string.Join(" ", messages)
            : "Некорректные входные данные.";

        return new BadRequestObjectResult(
            new ErrorResponse("validation_error", message, context.HttpContext.TraceIdentifier));
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Добрые лапки API",
        Version = "v1",
        Description = "REST API приюта животных «Добрые лапки»."
    });

    var presentationXml = Path.Combine(
        AppContext.BaseDirectory,
        $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(presentationXml))
    {
        options.IncludeXmlComments(presentationXml);
    }

    var domainXml = Path.Combine(AppContext.BaseDirectory, "DobreLapki.Domain.xml");
    if (File.Exists(domainXml))
    {
        options.IncludeXmlComments(domainXml);
    }
});

builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<IAnimalRepository, InMemoryAnimalRepository>();
builder.Services.AddSingleton<IApplicationRepository, InMemoryApplicationRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<IFavoritesRepository, InMemoryFavoritesRepository>();
builder.Services.AddSingleton<IFileStorage, MockFileStorage>();

builder.Services.AddScoped<IAnimalService, AnimalService>();
builder.Services.AddScoped<IApplicationService, ApplicationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IFavoriteService, FavoriteService>();

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Добрые лапки API v1");
        options.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.Run();

/// <summary>Точка входа приложения, открытая для интеграционных тестов.</summary>
public partial class Program
{
}
