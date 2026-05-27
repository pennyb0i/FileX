using FileX.Models;

namespace FileX.Services.Abstractions;

public interface IFileService
{
    /// <summary>
    /// Retrieves a snapshot of files and directories from the specified root path.
    /// </summary>
    /// <param name="rootPath">The root path of the directory to perform the file and directory snapshot.</param>
    /// <returns>A <see cref="Snapshot"/> object representing the current state of files and directories at the specified root path.</returns>
    Task<Snapshot> GetFiles(string rootPath);
}