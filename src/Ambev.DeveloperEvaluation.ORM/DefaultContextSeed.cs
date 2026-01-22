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
        await SeedDefaultUser(context, passwordHasher);
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

    private static async Task SeedDefaultUser(DefaultContext context, IPasswordHasher passwordHasher)
    {
        const string defaultEmail = "user@local.com";
        const string defaultPassword = "default@123";

        var existingUser = context.Users.FirstOrDefault(user => user.Email == defaultEmail);
        if (existingUser is not null)
        {
            return;
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "Default User",
            Email = defaultEmail,
            Phone = "(11) 99999-9999",
            Password = passwordHasher.HashPassword(defaultPassword),
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        await context.Users.AddAsync(user);
    }
}
