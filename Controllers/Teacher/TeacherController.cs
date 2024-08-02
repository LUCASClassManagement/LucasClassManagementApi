using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;
using System.Security.Claims;

namespace LucasClassManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeacherController : ControllerBase {
    private readonly TeacherDataBaseContext _teacherContext;

    //Constructeur de notre classe
    public TeacherController(TeacherDataBaseContext teacherContext) {
        _teacherContext = teacherContext ?? throw new ArgumentNullException(nameof(teacherContext));
    }

    //Requete pour prendre tous les utilisateurs (Teacher).
    [HttpGet]
    public IEnumerable<Teacher> GetAllTeachers() {
        return _teacherContext.Teachers.ToList();
    }

    //Requete pour prendre un utilisateur par son id (teacher).
    [HttpGet("{id}")]
    public IActionResult GetById(int id) {
        var teacherId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        /* Politique pour verifier si l'utilisateur qui fait la requete de cet utilisateur sont égaux,donc en résumé un autre utilisateur ne peut pas formuler la requete pour prendre un autre utilisateur */
        if(teacherId == null || teacherId != id.ToString()) {
            return Forbid();
        }

        var teacher = _teacherContext.Teachers.Find(id);

        if(teacher == null) {
            return Forbid();
        }

        return Ok(teacher);
    }

    //Requete pour créer un nouveau utilisateur (teacher), nous allons aussi mettre la policy à admin
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public IActionResult Add([FromBody] Teacher teacher) {
        if(teacher == null) {
            return BadRequest("data can't be null");
        }

        if(teacher.Password == null) {
            return BadRequest("Password can't be null");
        }

        teacher.Password = HashPassword(teacher.Password);

        _teacherContext.Teachers.Add(teacher);
        _teacherContext.SaveChanges();

        return CreatedAtAction(nameof(GetById), new {id = teacher.Id}, teacher);
    }

    //Requete pour modifier un utilisateur (teacher), nous n'allons pas mettre de policy pour permettre aux utilisateurs de modifier eux memes certaines informations
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] Teacher updateTeacher, int id) {
        var teacher = _teacherContext.Teachers.Find(id);

        if(teacher == null) {
            return NotFound();
        }

        if(updateTeacher.Password == null) {
            return BadRequest("Password can't be null");
        }

        teacher.Password = HashPassword(updateTeacher.Password);
        teacher.FirstName = updateTeacher.FirstName;
        teacher.Name = updateTeacher.Name;
        teacher.PhoneNumber = updateTeacher.PhoneNumber;

        _teacherContext.Teachers.Update(teacher);
        _teacherContext.SaveChanges();

        return NoContent();
    }

    //Requete pour supprimer un utilisateur (teacher), la policy reviendra à l'admin
    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult Delete(int id) {
        var teacher = _teacherContext.Teachers.Find(id);

        if(teacher == null) {
            return BadRequest();
        }

        _teacherContext.Teachers.Remove(teacher);
        _teacherContext.SaveChanges();

        return NoContent();
    }

     //Creation d'une fonction qui retourne la fonction de hashing du mot de passe par BCrypt
    private string HashPassword(string password) {
        if(password == null) {
            throw new ArgumentNullException(nameof(password), "Password can't be null");
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }
}