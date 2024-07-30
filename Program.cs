using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LucasClassManagementApi.Data;

namespace LucasClassManagementApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var studentConnection = builder.Configuration.GetConnectionString("StudentConnection")
                ?? throw new ArgumentNullException("StudentConnection", "StudentConnection is not configured");

            var teacherConnection = builder.Configuration.GetConnectionString("TeacherConnection")
                ?? throw new ArgumentNullException("TeacherConnection", "TeacherConnection is not configured");

            builder.Services.AddDbContext<StudentDataBaseContext>(options => 
                    options.UseSqlServer(studentConnection));

            builder.Services.AddDbContext<TeacherDataBaseContext>(options =>
                    options.UseSqlServer(teacherConnection));

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
