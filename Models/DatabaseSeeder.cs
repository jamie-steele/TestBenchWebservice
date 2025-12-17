using Microsoft.EntityFrameworkCore;
using TestBenchWebService.Services;

namespace TestBenchWebService.Models
{
    public static class DatabaseSeeder
    {
        public static async Task SeedDatabasesAsync(IServiceProvider serviceProvider)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<IBlogSeeder>();

                await SeedDatabaseAsync(scope.ServiceProvider.GetRequiredService<PostgresDbContextProvider>(), seeder, "PostgreSQL");
                await SeedDatabaseAsync(scope.ServiceProvider.GetRequiredService<MySqlDbContextProvider>(), seeder, "MySQL/MariaDB");
                await SeedDatabaseAsync(scope.ServiceProvider.GetRequiredService<SqlServerDbContextProvider>(), seeder, "SQL Server");
                await SeedDatabaseAsync(scope.ServiceProvider.GetRequiredService<OracleDbContextProvider>(), seeder, "Oracle");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in database seeding: {ex.Message}");
            }
        }

        private static async Task SeedDatabaseAsync<TContext>(
            IDynamicDbContextProvider<TContext> provider,
            IBlogSeeder seeder,
            string dbName)
            where TContext : DbContext
        {
            try
            {
                await using var context = provider.Create();
                if (context is null)
                {
                    Console.WriteLine($"Skipping {dbName} seeding - not configured");
                    return;
                }

                if (!await context.Database.CanConnectAsync())
                {
                    Console.WriteLine($"Skipping {dbName} seeding - database not available");
                    return;
                }

                await seeder.EnsureSeededAsync(context);
                Console.WriteLine($"Successfully ensured seed for {dbName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding {dbName}: {ex.Message}");
            }
        }
    }
}


