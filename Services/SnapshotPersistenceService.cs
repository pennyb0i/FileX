using System.Text.Json;
using FileX.Models;
using FileX.Services.Abstractions;

namespace FileX.Services;

public class SnapshotPersistenceService : ISnapshotPersistenceService
{
    private const string SnapshotDirectory = "Snapshots";
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public SnapshotPersistenceService(JsonSerializerOptions jsonSerializerOptions)
    {
        _jsonSerializerOptions = jsonSerializerOptions;
    }

    public async Task SaveSnapshotAsync(Snapshot snapshot)
    {
        if (!Directory.Exists(SnapshotDirectory))
        {
            Directory.CreateDirectory(SnapshotDirectory);
        }

        var sanitizedPath = string.Join("_", snapshot.RootPath.Split(Path.GetInvalidFileNameChars()));
        var filePath = Path.Combine(SnapshotDirectory, $"snapshot_{sanitizedPath}.json");

        var json = JsonSerializer.Serialize(snapshot, _jsonSerializerOptions);
        await File.WriteAllTextAsync(filePath, json);
    }

    public async Task<Snapshot?> GetSnapshotAsync(string rootPath)
    {
        var sanitizedPath = string.Join("_", rootPath.Split(Path.GetInvalidFileNameChars()));
        var filePath = Path.Combine(SnapshotDirectory, $"snapshot_{sanitizedPath}.json");

        if (!File.Exists(filePath))
        {
            return null;
        }

        var json = await File.ReadAllTextAsync(filePath);
        return JsonSerializer.Deserialize<Snapshot>(json, _jsonSerializerOptions);
    }
}
