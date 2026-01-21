using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sale item.
/// </summary>
public sealed class SaleItem : AggregateRoot
{
    /// <summary>
    /// Represents the product identifier.
    /// </summary>
    public Guid ProductId { get; private set; }

    /// <summary>
    /// Gets the quantity of the product.
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Gets the discount applied to the product.
    /// </summary>
    public decimal Discount { get; private set; }

    /// <summary>
    /// Gets the total price of the product after applying the discount.
    /// </summary>
    public decimal Total { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the sale item is cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    /// <summary>
    /// Constructor required for EF Core.
    /// </summary>
    private SaleItem() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItem"/> class with the specified parameters.
    /// </summary>
    /// <param name="productId">The product identifier.</param>
    /// <param name="quantity">The quantity of products.</param>
    /// <param name="unitPrice">The product unit price.</param>
    /// <param name="discount">The discount applied.</param>
    /// <param name="total">The total price after applying the discount.</param>
    public SaleItem(
        Guid productId,
        int quantity,
        decimal unitPrice,
        decimal discount,
        decimal total) : base(Guid.NewGuid())
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Total = total;
        IsCancelled = false;
    }

    /// <summary>
    /// Cancels the sale item.
    /// </summary>
    internal void Cancel()
    {
        if (IsCancelled)
        {
            return;
        }

        IsCancelled = true;
        Raise(new SaleItemCancelledEvent(this));
    }

    internal void UpdatePricing(int quantity, decimal unitPrice, decimal discount, decimal total)
    {
        if (IsCancelled)
        {
            throw new DomainException("Cannot update a cancelled sale item.");
        }

        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = discount;
        Total = total;
    }
}
