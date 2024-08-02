using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using LucasClassManagementApi.Models;

namespace LucasClassManagementApi.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<StudentDataBaseContext>
{
    public StudentDataBaseContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("StudentConnection");

        if(string.IsNullOrEmpty(connectionString)) {
            throw new InvalidOperationException("Connection string 'StudentConnection' Not found.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<StudentDataBaseContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new StudentDataBaseContext(optionsBuilder.Options);
    }
}

public class StudentDataBaseContext : DbContext {
    public StudentDataBaseContext(DbContextOptions<StudentDataBaseContext> options) : base(options) { }
    public DbSet<Student> Students { get; set; } = null!;
}

public class TeacherDataBaseContext : DbContext {
    public TeacherDataBaseContext(DbContextOptions<TeacherDataBaseContext> options) : base(options) { }
    public DbSet<Teacher> Teachers { get; set; } = null!;
}

public class ModuleDataBaseContext : DbContext {
    public ModuleDataBaseContext(DbContextOptions<ModuleDataBaseContext> options) : base(options) { }
    public DbSet<Module> Modules { get; set; } = null!;
}