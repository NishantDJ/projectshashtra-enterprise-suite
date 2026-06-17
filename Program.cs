using ProjectShashtra.Data;
using Microsoft.Data.SqlClient;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ProjectShashtra.Services;
using Serilog; // make sure this exists
using Microsoft.EntityFrameworkCore;
using ProjectShashtra.Data;

namespace ProjectShashtra
{
    public class Program
    {
        public static void Main(string[] args)
        {


            var builder = WebApplication.CreateBuilder(args);
            
            //Serilog Configuration
            var Configuration = new ConfigurationBuilder()
                //.AddJsonFile("appsettings.json",optional:false,reloadOnChange:true)
                .AddJsonFile("serilogsettings.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                 .Enrich.WithMachineName()
               .Enrich.WithEnvironmentName()
                .CreateLogger();

            builder.Host.UseSerilog();


            // JWT Configuration
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"], // ✅ FIXED TYPO
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };
            });

            // Database Connection Test
            var connectionString = builder.Configuration.GetConnectionString("DBCS");
            //using (SqlConnection con = new SqlConnection(connectionString))
            //{
            //    con.Open();
            //    Console.WriteLine("Database Connected Successfully");
            //}
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            // Add services to DI container
            builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    });

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<AuthService>(); 

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReact", policy =>
                {
                    policy.WithOrigins("http://localhost:5173") // Vite default port
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });
            var app = builder.Build();

            try
            {

                Log.Information("Starting web host");
                Log.Information(
                   "Starting web host on Machine: {MachineName}, Environment: {EnvironmentName}",
                   Environment.MachineName,
                   app.Environment.EnvironmentName);
                // Middleware pipeline
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();


                app.UseMiddleware<ExceptionMiddleware>();

                // ── in pipeline ──
                app.UseCors("AllowReact");
                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}