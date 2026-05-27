using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using FileX.Models;
using FileX.Services.Abstractions;

namespace FileX.Services;

public class FileService : IFileService
{
    private readonly FileScanSettings _settings;
    private readonly ILogger<FileService> _logger;

    public FileService(IOptions<FileScanSettings> settings, ILogger<FileService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public async Task<Snapshot> GetFiles(string rootPath)
    {
        var maxFileCount = _settings.MaxFileCount;
        var maxTotalSizeInBytes = _settings.MaxTotalSizeInBytes;
        var now = DateTime.UtcNow;
        var snapshot = new Snapshot
        {
            RootPath = rootPath,
            CreatedAtUtc = now
        };
        
        var files = new List<string>();
        try
        {
            files = GetAllFiles(rootPath).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical error occurred while scanning files at {Path}", rootPath);
            throw;
        }

        if (files.Count > maxFileCount)
        {
            throw new InvalidOperationException($"The folder contains more than {maxFileCount} files.");
        }

        long totalSize = 0;
        var accessibleFiles = new List<string>();
        foreach (var file in files)
        {
            try
            {
                totalSize += new FileInfo(file).Length;
                accessibleFiles.Add(file);
            }
            catch
            {
                // Skip files we can't access
            }
        }

        if (totalSize > maxTotalSizeInBytes)
        {
            throw new InvalidOperationException($"The folder size exceeds {maxTotalSizeInBytes / 1024 / 1024}MB.");
        }

        var fileTasks = accessibleFiles.Select(file => GetFileSnapshotsAsync(rootPath, file));
        var results = await Task.WhenAll(fileTasks);
        snapshot.Files.AddRange(results.Where(x => x != null).Cast<FileSnapshot>());

        snapshot.Directories = GetDirSnapshotsForPath(rootPath);

        return snapshot;
    }

    #region Private helper methods

    private static async Task<FileSnapshot?> GetFileSnapshotsAsync(string rootPath, string file)
    {
        try
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
        catch
        {
            return null;
        }
    }
    
    private static IEnumerable<string> GetAllFiles(string rootPath)
    {
        var files = new List<string>();
        var directories = new Queue<string>();
        directories.Enqueue(rootPath);

        while (directories.Count > 0)
        {
            var currentDir = directories.Dequeue();

            try
            {
                files.AddRange(Directory.EnumerateFiles(currentDir));
                foreach (var dir in Directory.EnumerateDirectories(currentDir))
                {
                    directories.Enqueue(dir);
                }
            }
            catch (UnauthorizedAccessException) { /* Skip */ }
            catch (PathTooLongException) { /* Skip */ }
            catch (DirectoryNotFoundException) { /* Skip */ }
        }

        return files;
    }

    private static List<DirectorySnapshot> GetDirSnapshotsForPath(string rootPath)
    {
        var dirs = new HashSet<string>();
        var directories = new Queue<string>();
        directories.Enqueue(rootPath);

        while (directories.Count > 0)
        {
            var currentDir = directories.Dequeue();
            try
            {
                foreach (var dir in Directory.EnumerateDirectories(currentDir))
                {
                    dirs.Add(Path.GetRelativePath(rootPath, dir));
                    directories.Enqueue(dir);
                }
            }
            catch (UnauthorizedAccessException) { /* Skip */ }
            catch (PathTooLongException) { /* Skip */ }
            catch (DirectoryNotFoundException) { /* Skip */ }
        }

        return dirs.Select(x => new DirectorySnapshot { RelativePath = x }).ToList();
    }

    #endregion
}
