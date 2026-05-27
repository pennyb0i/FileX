using FileX.Models;

namespace FileX.Services.Abstractions;

public interface ISnapshotPersistenceService
{
    /// <summary>
    /// Saves the provided snapshot to persistent storage.
    /// </summary>
    /// <param name="snapshot">The snapshot to save.</param>
    Task SaveSnapshotAsync(Snapshot snapshot);

    /// <summary>
    /// Retrieves a snapshot from persistent storage for the specified root path.
    /// </summary>
    /// <param name="rootPath">The root path associated with the snapshot.</param>
    /// <returns>A <see cref="Snapshot"/> object if found; otherwise, null.</returns>
    Task<Snapshot?> GetSnapshotAsync(string rootPath);
}
