using FileX.Models;

namespace FileX.Services.Abstractions;

public interface ISnapshotPersistenceService
{
    Task SaveSnapshotAsync(Snapshot snapshot);
    Task<Snapshot?> GetSnapshotAsync(string rootPath);
}
