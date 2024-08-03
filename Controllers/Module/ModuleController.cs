using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;

namespace LucasClassManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]

//Création d'un controller pour gérer les réquetes liés à Modules
public class ModuleController : ControllerBase {
    //Instanciation du contexte de base de donnée
    private readonly ModuleDataBaseContext _moduleContext;

    //Constructeur du controlleur
    public ModuleController(ModuleDataBaseContext moduleContext) {
        _moduleContext = moduleContext;
    }

    //Requete Get pour récupérer tous les Modules
    [HttpGet]
    public IEnumerable<Module> GetAllModules() {
        return _moduleContext.Modules.ToList();
    }

    //Requete Get pour récupérer un Module par son id, Policy => Admin
    [HttpGet("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult GetById(int id) {
        var module = _moduleContext.Modules.Find(id);
        
        //vérification de nullabilité
        if(module == null)  return Forbid();

        return Ok(module);
    }

    //Requete Post pour créer un nouveau Module, Policy => Admin
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public IActionResult Create([FromBody] Module module) {
        //vérification de nullabilité
        if(module == null)  return BadRequest();

        //Ajout et enregistrement
        _moduleContext.Modules.Add(module);
        _moduleContext.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = module.Id}, module);
    }

    //Requete Put pour modifier un Module, Policy => Admin
    [HttpPut("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult Update([FromBody] Module updateModule, int id) {
        var module = _moduleContext.Modules.Find(id);

        //vérification de nullabilité
        if(module == null)  return NotFound();

        module.ModuleName = updateModule.ModuleName;
        module.ModuleTimeTarget = updateModule.ModuleTimeTarget;

        //Mise à jour et enregistrement
        _moduleContext.Modules.Update(module);
        _moduleContext.SaveChanges();

        return NoContent();
    }

    //Requete Delete pour supprimer un module, Policy => Admin
    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult Delete(int id) {
        var module = _moduleContext.Modules.Find(id);

        //vérification de nullabilité
        if(module == null)  return NotFound();

        //Suppression de enregistrement
        _moduleContext.Modules.Remove(module);
        _moduleContext.SaveChanges();

        return NoContent();
    }
}