using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;
using System.Security.Claims;

namespace LucasClassManagementApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentController : ControllerBase {
    private readonly StudentDataBaseContext _studentContext;

    //Constructeur de notre classe
    public StudentController(StudentDataBaseContext studentContext) {
        _studentContext = studentContext ?? throw new ArgumentNullException(nameof(studentContext));
    }

    //Requete pour prendre tous les utilisateurs (students).
    [HttpGet]
    public IEnumerable<Student> GetAllStudents() {
        return _studentContext.Students.ToList();
    }

    //Requete pour prendre un utilisateur par son id (students).
    [HttpGet("{id}")]
    public IActionResult GetById(int id) {
        var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        /* Politique pour verifier si l'utilisateur qui fait la requete de cet utilisateur sont égaux,donc en résumé un autre utilisateur ne peut pas formuler la requete pour prendre un autre utilisateur */
        if(studentId == null || studentId != id.ToString()) {
            return Forbid();
        }

        var student = _studentContext.Students.Find(id);

        if(student == null) {
            return NotFound();
        }

        return Ok(student);
    }

    //Requete pour créer un nouveau utilisateur (student), nous allons aussi mettre la policy à admin
    [HttpPost]
    [Authorize(Policy = "Admin")]
    public IActionResult Add([FromBody] Student student) {
        if(student == null) {
            return BadRequest("data can't be null");
        }

        if(student.Password == null) {
            return BadRequest("Password can't be null");
        }

        student.Password = HashPassword(student.Password);

        _studentContext.Students.Add(student);
        _studentContext.SaveChanges();

        return CreatedAtAction(nameof(GetById), new {id = student.Id}, student);
    }

    //Requete pour modifier un utilisateur (student), nous n'allons pas mettre de policy pour permettre aux utilisateurs de modifier eux memes certaines informations
    [HttpPut("{id}")]
    public IActionResult Update([FromBody] Student updateStudent, int id) {
        var student = _studentContext.Students.Find(id);

        if(student == null) {
            return NotFound();
        }

        if(updateStudent.Password == null) {
            return BadRequest("Password can't be null");
        }

        student.Password = HashPassword(updateStudent.Password);
        student.FirstName = updateStudent.FirstName;
        student.Name = updateStudent.Name;
        student.PhoneNumber = updateStudent.PhoneNumber;

        _studentContext.Students.Update(student);
        _studentContext.SaveChanges();

        return NoContent();
    }

    //Requete pour supprimer un utilisateur (student), la policy reviendra à l'admin

    [HttpDelete("{id}")]
    [Authorize(Policy = "Admin")]
    public IActionResult Delete(int id) {
        var student = _studentContext.Students.Find(id);

        if(student == null) {
            return NotFound();
        }

        _studentContext.Students.Remove(student);
        _studentContext.SaveChanges();

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