using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LucasClassManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]

//Création d'un controlleur pour gérer les requetes et gestion de fileType de Course
public class CourseUploadController : ControllerBase {
    //instanciation du stockeur de fichier
    private readonly string _targetFilePathPdf;
    private readonly string _targetFilePathDocx;
    private readonly string _targetFilePathPptx;
    private readonly string _targetFilePathXlsx;
    private readonly string _targetFilePathTxt;

    public CourseUploadController(IWebHostEnvironment env) {
        _targetFilePathPdf = Path.Combine(env.WebRootPath, "files/PDF");
        _targetFilePathDocx = Path.Combine(env.WebRootPath, "files/DOCUMENT_WORD");
        _targetFilePathPptx = Path.Combine(env.WebRootPath, "files/POWER_POINT");
        _targetFilePathXlsx = Path.Combine(env.WebRootPath, "files/EXCEL");
        _targetFilePathTxt = Path.Combine(env.WebRootPath, "files/TEXTE");
    }

    //Post, pour uploader un ficher Course
    [HttpPost("upload")]
    public async Task<IActionResult> Upload(IFormFile file) {
        if(file == null || file.Length == 0)    return BadRequest("No file uploaded");
        else    Console.WriteLine("File Uploaded");

        var AllowedExtentions = new[] {".pdf", ".docx", ".pptx", ".xlsx", ".txt"};
        var extention = Path.GetExtension(file.FileName).ToLowerInvariant();

        if(!AllowedExtentions.Contains(extention))  return BadRequest("Unsupported extension");

        var path = extention switch {
            ".pdf" => Path.Combine(_targetFilePathPdf, file.FileName),
            ".docx" => Path.Combine(_targetFilePathDocx, file.FileName),
            ".pptx" => Path.Combine(_targetFilePathPptx, file.FileName),
            ".xlsx" => Path.Combine(_targetFilePathXlsx, file.FileName),
            ".txt" => Path.Combine(_targetFilePathTxt, file.FileName),
            _ => null
        };

        if(path == null)    return BadRequest("invalid file path");

        var directoryPath = Path.GetDirectoryName(path);

        if(directoryPath == null)   return StatusCode(StatusCodes.Status500InternalServerError, "Invalid directory path");

        if(!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

        using (var stream = new FileStream(path, FileMode.Create)) {
            await file.CopyToAsync(stream);
        }

        return Ok(new { filePath = path });
    }

    //Requete Get
    [HttpGet("files")]
    public IActionResult GetFiles() {
         // Créer une liste pour stocker les noms de fichiers
        var files = new List<string>();

        // Vérifier et ajouter les fichiers PDF
        if (Directory.Exists(_targetFilePathPdf))
        {
            files.AddRange(Directory.GetFiles(_targetFilePathPdf).Select(f => Path.GetFileName(f)));
        }

        // Vérifier et ajouter les fichiers Word
        if (Directory.Exists(_targetFilePathDocx))
        {
            files.AddRange(Directory.GetFiles(_targetFilePathDocx).Select(f => Path.GetFileName(f)));
        }

        // Vérifier et ajouter les fichiers PowerPoint
        if (Directory.Exists(_targetFilePathPptx))
        {
            files.AddRange(Directory.GetFiles(_targetFilePathPptx).Select(f => Path.GetFileName(f)));
        }

        // Vérifier et ajouter les fichiers Excel
        if (Directory.Exists(_targetFilePathXlsx))
        {
            files.AddRange(Directory.GetFiles(_targetFilePathXlsx).Select(f => Path.GetFileName(f)));
        }

        // Vérifier et ajouter les fichiers Textes
        if (Directory.Exists(_targetFilePathTxt))
        {
            files.AddRange(Directory.GetFiles(_targetFilePathTxt).Select(f => Path.GetFileName(f)));
        }

        // Retourner les fichiers concaténés
        return Ok(files);
    }
}