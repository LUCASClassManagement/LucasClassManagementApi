using Microsoft.AspNetCore.Identity.Data;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Models;

namespace LucasClassManagementApi.Services;

public class AuthService {
    private readonly TeacherDataBaseContext _teacherContext = null!;
    private readonly StudentDataBaseContext _studentContext = null!;
    private readonly IConfiguration _configuration = null!;

    public AuthService(IConfiguration configuration, TeacherDataBaseContext teacherContext, StudentDataBaseContext studentContext) {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _teacherContext = teacherContext ?? throw new ArgumentNullException(nameof(teacherContext));
            _studentContext = studentContext ?? throw new ArgumentNullException(nameof(studentContext));
    }

    //Création d'un service d'où va puiser les deux models pour le controlleur d'authentification

    //instance de GenerateJwtToken pour Teacher
    public string GenerateJwtToken(Teacher teacher) {
        if(teacher == null) throw new ArgumentNullException(nameof(teacher));
        return GenerateJwtToken(teacher.TeacherNumber, teacher.Role, teacher.Id.ToString());
    }

    //instance de GenerateJwtToken pour Student
    public string GenerateJwtToken(Student student) {
        if(student == null) throw new ArgumentNullException(nameof(student));
        return GenerateJwtToken(student.StudentNumber, student.Role, student.Id.ToString());
    }

    public bool ValidateTeacherCredential(string teacherNumber, string password, out Teacher teacher) {
        teacher = _teacherContext.Teachers.SingleOrDefault(u => u.TeacherNumber == teacherNumber);

        if(teacher == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, teacher.Password);
    }

    public bool ValidateStudentCredential(string studentNumber, string password, out Student student) {
        student = _studentContext.Students.SingleOrDefault(u => u.StudentNumber == studentNumber);

        if(student == null) return false;

        return BCrypt.Net.BCrypt.Verify(password, student.Password);
    }

    //Création du système d'authentification de json web token
    private string GenerateJwtToken(string? userNumber, string? role, string userId) {
        if(string.IsNullOrEmpty(userNumber)) throw new ArgumentNullException(nameof(userNumber));
        if(string.IsNullOrEmpty(role)) throw new ArgumentNullException(nameof(role));

        var jwtKey = _configuration["Jwt:Key"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];

        if(string.IsNullOrEmpty(jwtKey)) throw new ArgumentNullException(nameof(jwtKey));
        if(string.IsNullOrEmpty(jwtIssuer)) throw new ArgumentException(nameof(jwtIssuer));
        if(string.IsNullOrEmpty(jwtAudience)) throw new ArgumentNullException(nameof(jwtAudience));

        if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience)) {
                throw new ApplicationException("JWT configuration is missing or invalid.");
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Claims = new[] {
            new Claim(JwtRegisteredClaimNames.Sub, userNumber),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("userId", userId)
        };

        //Création du token d'une durée de vie de 2h
        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: Claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}