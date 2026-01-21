using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

public class SaleRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_ShouldIncludeCancelledItems()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase($"sales-{Guid.NewGuid()}")
            .Options;

        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();

        await using var context = new DefaultContext(options);
        context.Users.Add(new User
        {
            Id = customerId,
            Username = "Customer Test",
            Email = "customer@test.com",
            Phone = "(11) 99999-9999",
            Password = "Pass@word1",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        });

        var branch = new Branch("Main Branch", "12345678901234") { Id = branchId };
        context.Branches.Add(branch);

        var sale = new Sale(Guid.NewGuid(), "SALE-1", DateTime.UtcNow, customerId, branchId);
        var productId = Guid.NewGuid();
        sale.AddItem(productId, 4, 10m);
        var item = sale.Items.Single(i => i.ProductId == productId);
        sale.CancelItem(item.Id);

        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        var repository = new SaleRepository(context);
        var loadedSale = await repository.GetByIdAsync(sale.Id);

        Assert.NotNull(loadedSale);
        Assert.Single(loadedSale!.Items);
        Assert.True(loadedSale.Items.Single().IsCancelled);
    }
}
