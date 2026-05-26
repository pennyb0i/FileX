using System.Security.Cryptography;
using FileX.Models;
using FileX.Services.Abstractions;

namespace FileX.Services;

public class FileService : IFileService
{
    public async Task<Snapshot> GetFiles(string rootPath)
    {
        var now = DateTime.UtcNow;
        var snapshot = new Snapshot
        {
            RootPath = rootPath,
            CreatedAtUtc = now
        };
        
        var files = Directory.EnumerateFiles(rootPath, "*", SearchOption.AllDirectories).ToList();
        var fileTasks = files.Select(file => GetFileSnapshotsAsync(rootPath, file));
        snapshot.Files.AddRange(await Task.WhenAll(fileTasks));

        snapshot.Directories = GetDirSnapshotsForPath(rootPath);

        return snapshot;
    }

    #region Private helper methods

    private static async Task<FileSnapshot> GetFileSnapshotsAsync(string rootPath, string file)
    {
        var relativePath = Path.GetRelativePath(rootPath, file)
            .Replace('\\', '/');
        var info = new FileInfo(file);
        
        await using var stream = File.OpenRead(file);
        var hash = await SHA256.HashDataAsync(stream);
        
        return new FileSnapshot
        {
            Hash = Convert.ToHexStringLower(hash),
            RelativePath = relativePath,
            Size = info.Length,
            LastModifiedUtc = info.LastWriteTimeUtc
        };
    }
    
    private static List<DirectorySnapshot> GetDirSnapshotsForPath(string rootPath)
    {
        var dirs = Directory
            .EnumerateDirectories(rootPath, "*", SearchOption.AllDirectories)
            .Select(d => Path.GetRelativePath(rootPath, d))
            .ToHashSet();

        return dirs.Select(x => new DirectorySnapshot { RelativePath = x }).ToList();
    }

    #endregion
}
