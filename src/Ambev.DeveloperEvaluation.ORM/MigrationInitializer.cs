using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ambev.DeveloperEvaluation.ORM;

/// <summary>
/// Represents the migration initializer for the ORM layer.
/// </summary>
public static class MigrationInitializer
{
    /// <summary>
    /// Applies pending migrations to the database.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    public static void ApplyMigrations(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DefaultContext>();
		try
		{
            Console.WriteLine("Applying migrations...");
            context.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
		catch (Exception ex)
		{
            Console.WriteLine($"Error applying migrations: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
		}        
    }
}
