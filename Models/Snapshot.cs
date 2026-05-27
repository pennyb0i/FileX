using FileX.Enums;

namespace FileX.Models;

public class Snapshot
{
    public string RootPath { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? PreviousSnapshotCreatedAtUtc { get; set; }
    public List<FileSnapshot> Files { get; set; } = [];
    public List<DirectorySnapshot> Directories { get; set; } = [];
}

public class FileSnapshot
{
    public string RelativePath { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public long Size { get; set; }
    public int Version { get; set; } = 1;
    public FileState State { get; set; } = FileState.New;
    public DateTime LastModifiedUtc { get; set; }
}

public class DirectorySnapshot
{
    public string RelativePath { get; set; } = string.Empty;
    public FileState State { get; set; } = FileState.Unchanged;
}