using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JiggswapMigrations
{
    public static class Program
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "<Pending>")]
        private static string GetConnectionString()
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var connectionString = config["ConnectionStrings:JiggswapDb"];

                return connectionString;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        public static void Main()
        {
            var connectionString = GetConnectionString();

            if (string.IsNullOrEmpty(connectionString))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please include `ConnectionStrings:JiggswapDb` key in appsettings.json");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            var serviceProvider = CreateServices(connectionString);

            // Put the database update into a scope to ensure
            // that all resources will be disposed.
            using var scope = serviceProvider.CreateScope();

            UpdateDatabase(scope.ServiceProvider);
        }

        private static void UpdateDatabase(IServiceProvider serviceProvider)
        {
            // Instantiate the runner
            var runner = serviceProvider.GetRequiredService<IMigrationRunner>();

            // Execute the migrations

            // runner.MigrateDown(202006121517);

            runner.MigrateUp();
        }

        private static IServiceProvider CreateServices(string connectionString)
        {
            return new ServiceCollection()
                  // Add common FluentMigrator services
                  .AddFluentMigratorCore()
                  .ConfigureRunner(rb => rb
                      // Add SQLite support to FluentMigrator
                      .AddPostgres()
                      // Set the connection string
                      .WithGlobalConnectionString(connectionString)
                      // Define the assembly containing the migrations
                      .ScanIn(typeof(Program).Assembly).For.Migrations())
                  // Enable logging to console in the FluentMigrator way
                  .AddLogging(lb => lb.AddFluentMigratorConsole())
                  // Build the service provider
                  .BuildServiceProvider(false);
        }
    }
}