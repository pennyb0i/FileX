using System.Diagnostics;
using FileX.Enums;
using Microsoft.AspNetCore.Mvc;
using FileX.Models;
using FileX.Services.Abstractions;
using Microsoft.Extensions.Logging;

namespace FileX.Controllers;

public class HomeController : Controller
{
    private readonly IFileService _fileService;
    private readonly ISnapshotPersistenceService _snapshotPersistenceService;
    private readonly ILogger<HomeController> _logger;
    
    public HomeController(IFileService fileService, ISnapshotPersistenceService snapshotPersistenceService, ILogger<HomeController> logger)
    {
        _fileService = fileService;
        _snapshotPersistenceService = snapshotPersistenceService;
        _logger = logger;
    }
    
    /// <summary>
    /// Displays the index page.
    /// </summary>
    /// <returns>The index view.</returns>
    public IActionResult Index()
    {
        return View();
    }
    
    /// <summary>
    /// Processes and displays a snapshot of the specified root path.
    /// </summary>
    /// <param name="rootPath">The root path to snapshoot.</param>
    /// <returns>The snapshot view.</returns>
    public async Task<IActionResult> ViewSnapshot(string rootPath)
    {
        try
        {
            var currentSnapshot = await _fileService.GetFiles(rootPath);
            var existingSnapshot = await _snapshotPersistenceService.GetSnapshotAsync(rootPath);

            currentSnapshot.ReconcileWith(existingSnapshot);

            var snapshotToSave = new Snapshot
            {
                RootPath = currentSnapshot.RootPath,
                CreatedAtUtc = currentSnapshot.CreatedAtUtc,
                Files = currentSnapshot.Files.Where(f => f.State != FileState.Deleted).ToList(),
                Directories = currentSnapshot.Directories.Where(d => d.State != FileState.Deleted).ToList()
            };
            
            await _snapshotPersistenceService.SaveSnapshotAsync(snapshotToSave);
            return View(currentSnapshot);
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "A critical error occurred while processing the snapshot.");
            TempData["ErrorMessage"] = "A critical error occurred while processing the snapshot.";
            return RedirectToAction("Index");
        }
    }

    /// <summary>
    /// Displays the error page.
    /// </summary>
    /// <returns>The error view.</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}