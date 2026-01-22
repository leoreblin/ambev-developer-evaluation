using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact]
    public void AddItem_ShouldApplyTenPercentDiscount_WhenQuantityBetweenFourAndNine()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        sale.AddItem(productId, "Product", 4, 10m);

        var item = sale.Items.Single(i => i.ProductId == productId);
        item.Discount.Should().Be(0.10m);
        item.Total.Should().Be(36m);
        sale.TotalAmount.Should().Be(36m);
    }

    [Fact]
    public void AddItem_ShouldApplyTenPercentDiscount_WhenQuantityIsNine()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        sale.AddItem(productId, "Product", 9, 10m);

        var item = sale.Items.Single(i => i.ProductId == productId);
        item.Discount.Should().Be(0.10m);
        item.Total.Should().Be(81m);
    }

    [Fact]
    public void AddItem_ShouldApplyTwentyPercentDiscount_WhenQuantityBetweenTenAndTwenty()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        sale.AddItem(productId, "Product", 10, 10m);

        var item = sale.Items.Single(i => i.ProductId == productId);
        item.Discount.Should().Be(0.20m);
        item.Total.Should().Be(80m);
        sale.TotalAmount.Should().Be(80m);
    }

    [Fact]
    public void AddItem_ShouldApplyTwentyPercentDiscount_WhenQuantityIsTwenty()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        sale.AddItem(productId, "Product", 20, 10m);

        var item = sale.Items.Single(i => i.ProductId == productId);
        item.Discount.Should().Be(0.20m);
        item.Total.Should().Be(160m);
    }

    [Fact]
    public void AddItem_ShouldNotApplyDiscount_WhenQuantityBelowFour()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        sale.AddItem(productId, "Product", 3, 10m);

        var item = sale.Items.Single(i => i.ProductId == productId);
        item.Discount.Should().Be(0m);
        item.Total.Should().Be(30m);
        sale.TotalAmount.Should().Be(30m);
    }

    [Fact]
    public void AddItem_ShouldThrow_WhenQuantityAboveTwenty()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();

        var act = () => sale.AddItem(productId, "Product", 21, 10m);

        act.Should().Throw<DomainException>()
            .WithMessage("Maximum 20 items per product allowed.");
    }

    [Fact]
    public void Cancel_ShouldCancelItemsAndZeroTotal()
    {
        var sale = CreateSale();
        sale.AddItem(Guid.NewGuid(), "Product A", 2, 10m);
        sale.AddItem(Guid.NewGuid(), "Product B", 4, 5m);

        sale.Cancel();

        sale.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(0m);
        sale.Items.All(i => i.IsCancelled).Should().BeTrue();
    }

    [Fact]
    public void CancelItem_ShouldUpdateTotal()
    {
        var sale = CreateSale();
        var productId = Guid.NewGuid();
        sale.AddItem(productId, "Product", 4, 10m);
        sale.AddItem(Guid.NewGuid(), "Product B", 2, 5m);

        var itemToCancel = sale.Items.Single(i => i.ProductId == productId);
        sale.CancelItem(itemToCancel.Id);

        sale.TotalAmount.Should().Be(10m);
    }

    private static Sale CreateSale()
        => new(
            Guid.NewGuid(),
            "SALE-TEST",
            DateTime.UtcNow,
            Guid.NewGuid(),
            "Customer",
            Guid.NewGuid(),
            "Branch");
}
