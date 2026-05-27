using FileX.Enums;

namespace FileX.Models;

public static class SnapshotExtensions
{
    public static void ReconcileWith(this Snapshot current, Snapshot? existing)
    {
        if (existing == null)
        {
            return;
        }

        current.PreviousSnapshotCreatedAtUtc = existing.CreatedAtUtc;
        
        foreach (var currentFile in current.Files)
        {
            var existingFile = existing.Files.FirstOrDefault(f => f.RelativePath == currentFile.RelativePath);
            if (existingFile != null)
            {
                if (existingFile.Hash != currentFile.Hash)
                {
                    currentFile.Version = existingFile.Version + 1;
                    currentFile.State = FileState.Modified;
                }
                else
                {
                    currentFile.Version = existingFile.Version;
                    currentFile.State = FileState.Unchanged;
                }
            }
            else
            {
                currentFile.State = FileState.New;
            }
        }

        foreach (var currentDir in current.Directories)
        {
            var existingDir = existing.Directories.FirstOrDefault(d => d.RelativePath == currentDir.RelativePath);
            if (existingDir is not null)
            {
                currentDir.State = FileState.Unchanged;
            }
        }

        var deletedFiles = existing.Files
            .Where(e => current.Files.All(c => c.RelativePath != e.RelativePath))
            .ToList();

        foreach (var deletedFile in deletedFiles)
        {
            deletedFile.State = FileState.Deleted;
            current.Files.Add(deletedFile);
        }

        var deletedDirectories = existing.Directories
            .Where(e => current.Directories.All(c => c.RelativePath != e.RelativePath))
            .ToList();

        foreach (var deletedDir in deletedDirectories)
        {
            deletedDir.State = FileState.Deleted;
            current.Directories.Add(deletedDir);
        }
    }
}
