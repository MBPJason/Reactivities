using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;

namespace API
{
    public class Program
    {
        // Turned into an async Task for the await calls in the the try and catch block
        public static async Task Main(string[] args)
        {
            // Stores 'host' value for later uses
            var host = CreateHostBuilder(args).Build();
            // Sets up a one time use for scope when running this function
            using var scope = host.Services.CreateScope();
            // Stores all services needed for database migration into 'services' variable
            var services = scope.ServiceProvider;

            try
            {   
                // Grab seed data of Activities
                var context = services.GetRequiredService<DataContext>();
                // Grab seed data of Users 
                var userManager = services.GetRequiredService<UserManager<AppUser>>();
                // If no database exists builds out database
                await context.Database.MigrateAsync();
                // Inject seed data, if needed
                await Seed.SeedData(context, userManager);
            }
            catch (Exception ex)
            {
                // Error logger 
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred during migration");
            }

            // Run the application
            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
