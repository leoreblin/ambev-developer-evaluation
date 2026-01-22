using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.ORM;

public static class DefaultContextSeed
{
    public static async Task SeedAsync(DefaultContext? context, IPasswordHasher passwordHasher)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(passwordHasher);

        await SeedDefaultBranches(context);
        await SeedDefaultUsers(context, passwordHasher);
        await context.SaveChangesAsync();
    }

    private static async Task SeedDefaultBranches(DefaultContext context)
    {
        if (!context.Branches.Any())
        {
            var branches = new List<Branch>
            {
                new("Ambev Branch I", "17757062000145"),
                new("Ambev Branch II", "49458248000190")
            };

            await context.Branches.AddRangeAsync(branches);
        }
    }

    private static async Task SeedDefaultUsers(DefaultContext context, IPasswordHasher passwordHasher)
    {
        const string defaultPassword = "default@123";

        if (context.Users.Any())
        {
            return;
        }

        List<User> users = [

            // Default Admin User
            new User()
            {
                Id = Guid.NewGuid(),
                Username = "Default Admin",
                Email = "admin@local.com",
                Phone = "(11) 99999-9999",
                Password = passwordHasher.HashPassword(defaultPassword),
                Role = UserRole.Admin,
                Status = UserStatus.Active
            },

            // Default Customer User
            new User()
            {
                Id = Guid.NewGuid(),
                Username = "Default Customer",
                Email = "customer@local.com",
                Phone = "(11) 99999-9999",
                Password = passwordHasher.HashPassword(defaultPassword),
                Role = UserRole.Customer,
                Status = UserStatus.Active
            }
        ];        

        await context.Users.AddRangeAsync(users);
    }
}
