using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;

namespace LucasClassManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NoteController : ControllerBase {
    //instanciation de la base de donnée
    private readonly NoteDataBaseContext _noteContext;

    //Constructeur de la classe
    public NoteController(NoteDataBaseContext noteContext) {
        _noteContext = noteContext;
    }

    //Requete GetAll
    [HttpGet]
    public IEnumerable<Note> GetAllNotes() {
        return _noteContext.Notes.ToList();
    }

    //Requete GetById
    [HttpGet("{id}")]
    public IActionResult GetById(int id) {
        var note = _noteContext.Notes.Find(id);

        if(note == null)    return BadRequest();

        return Ok(note);
    }

    //Requete Post, Policy => "Admin"
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public IActionResult CreateNote([FromBody] Note note) {
        if(note == null)     return BadRequest();

        //Ajout et enregistrement
        _noteContext.Notes.Add(note);
        _noteContext.SaveChanges();

        return CreatedAtAction(nameof(GetById), new{ id = note.Id }, note);
    }

    //Requete Put, Policy => Admin
    [HttpPut("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult UpdateNote([FromBody] Note updateNote, int id) {
        var note = _noteContext.Notes.Find(id);

        if(note == null)    return NotFound();

        //itération des modifications
        note.ModuleName = updateNote.ModuleName;
        note.ModuleApreciation = updateNote.ModuleApreciation;
        note.ModuleNote = updateNote.ModuleNote;

        //application et enregistrement
        _noteContext.Notes.Update(note);
        _noteContext.SaveChanges();

        return NoContent();
    }

    //Requete Delete, policy => Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult DeleteNote(int id) {
        var note = _noteContext.Notes.Find(id);

        if(note == null)    return NotFound();

        //Supression et enregistrement
        _noteContext.Notes.Remove(note);
        _noteContext.SaveChanges();

        return NoContent();
    }

}