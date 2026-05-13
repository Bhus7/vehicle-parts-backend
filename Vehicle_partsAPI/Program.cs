using System;
using Microsoft.EntityFrameworkCore;
using vehicle_parts.Data;
using vehicle_parts.Repositories;
using vehicle_parts.Services;
namespace vehicle_parts
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            builder.Services.AddScoped<IStaffRepository, StaffRepository>();
            builder.Services.AddScoped<IStaffService, StaffService>();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            
            // Register Background Service for Automatic Notifications
            builder.Services.AddHostedService<NotificationBackgroundService>();


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            var app = builder.Build();

            // Check database connection on startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    // Try to open the connection to get the actual error message
                    context.Database.OpenConnection();
                    Console.WriteLine("Database connected successfully");
                    context.Database.CloseConnection();

                    // Seed Roles if they don't exist
                    if (!context.Roles.Any())
                    {
                        context.Roles.AddRange(
                            new vehicle_parts.Models.Role { RoleName = "ADMIN" },
                            new vehicle_parts.Models.Role { RoleName = "STAFF" }
                        );
                        context.SaveChanges();
                        Console.WriteLine("Default roles (ADMIN, STAFF) seeded.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("DATABASE CONNECTION ERROR:");
                    Console.WriteLine(ex.Message);
                    Console.WriteLine("--------------------------------------------------");
                    Environment.Exit(1);
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
