using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FileX.Models;
using FileX.Services.Abstractions;

namespace FileX.Controllers;

public class HomeController : Controller
{
    private readonly IFileService _fileService;
    
    public HomeController(IFileService fileService)
    {
        _fileService = fileService;
    }
    
    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> ViewSnapshot(string rootPath)
    {
        var files = await _fileService.GetFiles(rootPath);
        return View(files);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}