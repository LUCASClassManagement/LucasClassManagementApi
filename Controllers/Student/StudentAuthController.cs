using Microsoft.AspNetCore.Mvc;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;
using LucasClassManagementApi.Services;

namespace LucasClassManagementApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class StudentAuthController : ControllerBase
{
    private readonly StudentDataBaseContext _studentContext;
    private readonly AuthService _authService;

    //Constructeur de la classe controlleur
    public StudentAuthController(StudentDataBaseContext stuentContext, AuthService authService) {
        _studentContext = stuentContext;   
        _authService = authService;

        if(!_studentContext.Students.Any()) {
            var admin = new Student {
                StudentNumber = "545",
                Password = BCrypt.Net.BCrypt.HashPassword("thiemokotraore545"),
                Role = "Admin"
            };
            _studentContext.Students.Add(admin);
            _studentContext.SaveChanges();
        }
    }

    //Controlleur pour la requete de connexion de l'utilisateur (student)
    [HttpPost("student_login")]
    public IActionResult Login([FromBody] StudentLoginRequest loginRequest) {
        var student = _studentContext.Students.SingleOrDefault(u => u.StudentNumber == loginRequest.StudentNumber);

        if(student == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, student.Password)) {
            return Unauthorized(new { message = "Invalid user number or password" });
        }

        var token = _authService.GenerateJwtToken(student);

        return Ok(new{token, role = student.Role});
    }
}