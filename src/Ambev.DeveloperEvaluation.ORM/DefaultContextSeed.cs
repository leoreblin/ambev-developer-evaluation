using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.ORM;

public static class DefaultContextSeed
{
    public static async Task SeedAsync(DefaultContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);

        await SeedDefaultBranches(context);
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
}
