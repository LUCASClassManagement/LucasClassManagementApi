using Microsoft.AspNetCore.Mvc;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;
using LucasClassManagementApi.Services;

namespace LucasClassManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
//Création d'une classe d'authentification qui va hériter de ControllerBase
public class TeacherAuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly TeacherDataBaseContext _teacherContext;

    //Constructeur du controlleur d'authentification
    public TeacherAuthController(AuthService authService, TeacherDataBaseContext teacherContext) {
        _authService = authService;
        _teacherContext = teacherContext;

        if(!_teacherContext.Teachers.Any()) {
            var admin = new Teacher {
                TeacherNumber = "1",
                Password = BCrypt.Net.BCrypt.HashPassword("mohamedlamine1"),
                Role = "Admin"
            };
            _teacherContext.Teachers.Add(admin);
            _teacherContext.SaveChanges();
        }
    }

    //Requete d'authentification de connexion pour l'utilisateur (Teacher)
    [HttpPost("teacher_login")]
    public IActionResult Login([FromBody] TeacherLoginRequest loginRequest) {
        var teacher = _teacherContext.Teachers.SingleOrDefault(u => u.TeacherNumber == loginRequest.TeacherNumber);

        if(teacher == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, teacher.Password)) {
            return Unauthorized(new { message = "Invalid user number or password" });
        }

        var token = _authService.GenerateJwtToken(teacher);

        return Ok(new{token, role = teacher.Role});
    }
}