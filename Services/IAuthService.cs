using LucasClassManagementApi.Models;

public interface IAuthService {
    string GenerateJwtToken(Teacher teacher);
    string GenerateJwtToken(Student student);
    bool ValidateTeacherCredential(string teacherNumber, string password, out Teacher teacher);
    bool ValidateStudentCredential(string studentNumber, string password, out Student student);
}