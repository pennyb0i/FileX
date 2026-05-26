using System.Diagnostics;
using FileX.Enums;
using Microsoft.AspNetCore.Mvc;
using FileX.Models;
using FileX.Services.Abstractions;

namespace FileX.Controllers;

public class HomeController : Controller
{
    private readonly IFileService _fileService;
    private readonly ISnapshotPersistenceService _snapshotPersistenceService;
    
    public HomeController(IFileService fileService, ISnapshotPersistenceService snapshotPersistenceService)
    {
        _fileService = fileService;
        _snapshotPersistenceService = snapshotPersistenceService;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> ViewSnapshot(string rootPath)
    {
        var currentSnapshot = await _fileService.GetFiles(rootPath);
        var existingSnapshot = await _snapshotPersistenceService.GetSnapshotAsync(rootPath);

        if (existingSnapshot != null)
        {
            foreach (var currentFile in currentSnapshot.Files)
            {
                var existingFile = existingSnapshot.Files.FirstOrDefault(f => f.RelativePath == currentFile.RelativePath);
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
            
            var deletedFiles = existingSnapshot.Files
                .Where(e => currentSnapshot.Files.All(c => c.RelativePath != e.RelativePath))
                .ToList();
            
            foreach (var deletedFile in deletedFiles)
            {
                deletedFile.State = FileState.Deleted;
                currentSnapshot.Files.Add(deletedFile);
            }
        }

        await _snapshotPersistenceService.SaveSnapshotAsync(currentSnapshot);
        return View(currentSnapshot);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}