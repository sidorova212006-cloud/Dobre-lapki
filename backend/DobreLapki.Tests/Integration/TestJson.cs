using System.Text.Json;
using System.Text.Json.Serialization;

namespace DobreLapki.Tests.Integration;

/// <summary>Настройки JSON для чтения ответов API в тестах (enum-ы строками).</summary>
internal static class TestJson
{
    public static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
}
