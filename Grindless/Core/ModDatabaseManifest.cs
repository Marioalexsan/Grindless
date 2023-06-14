using Microsoft.Extensions.Logging;
using RestSharp;
using System.Text.Json;

namespace Grindless.Core;

internal static class ModDatabaseConfig
{
    public const string ManifestLocation = "https://raw.githubusercontent.com/Marioalexsan/Grindless/main/ModDatabaseManifest.json";
}

internal class DownloadLink
{
    public string Url { get; set; }
    public string Sha1 { get; set; }
}

internal class ModDatabaseEntry
{
    public string DisplayName { get; set; }
    public string Description { get; set; }
    public Version Version { get; set; }
    public DownloadLink AssemblyLink { get; set; }
}

internal class ModDatabaseManifest
{
    public Version ManifestVersion { get; set; } = new(0, 1);

    public Dictionary<string, ModDatabaseEntry> Entries { get; set; } = new();

    public static ModDatabaseManifest FetchManifest()
    {
        using var client = new RestClient();

        var response = client.Get(new(ModDatabaseConfig.ManifestLocation));

        if (!response.IsSuccessful)
        {
            Program.Logger.LogWarning("Failed to fetch mod database manifest!");
            return new();
        }

        return JsonSerializer.Deserialize<ModDatabaseManifest>(response.Content);
    }
}