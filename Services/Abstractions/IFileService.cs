using FileX.Models;

namespace FileX.Services.Abstractions;

public interface IFileService
{
    Task<Snapshot> GetFiles(string rootPath);
}