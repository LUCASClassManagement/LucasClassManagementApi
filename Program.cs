using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LucasClassManagementApi.Data;
using LucasClassManagementApi.Services;

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

            //add the service scoped
            builder.Services.AddScoped<AuthService>();

            // Add services to the container.

            builder.Services.AddControllers();

            //Configuration of jason web token
            var jwtkey = builder.Configuration["jwt:key"]
                    ?? throw new ArgumentNullException("Jwt:Key", "Jwt:Key is not configured");
            
            if(jwtkey.Length < 32) {
                throw new ArgumentNullException("Jwt:Key must be at least 32 character long.");
            }

            var jwtissuer = builder.Configuration["jwt:Issuer"]
                ?? throw new ArgumentNullException("Jwt:Issuer", "Jwt:Issuer is not configured");

            var jwtAudience = builder.Configuration["jwt:Audience"]
                ?? throw new ArgumentNullException("Jwt:Audience", "Jwt:Audience is not configured");
            
            var Key = Encoding.UTF8.GetBytes(jwtkey);

            //Adding Authentication
            builder.Services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options => {
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtissuer,
                    ValidAudience = jwtAudience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtkey))
                };
            });

            //Adding Authorization
            builder.Services.AddAuthorization(options => {
                options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                options.AddPolicy("Student", policy => policy.RequireRole("Student"));
            });

            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin",
                builder => builder.WithOrigins("http://localhost:5173")
                                  .AllowAnyHeader()
                                  .AllowAnyMethod()
                                  .AllowCredentials());
            });

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
            
            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
