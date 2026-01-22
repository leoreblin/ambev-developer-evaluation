using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : AggregateRoot
{
    /// <summary>
    /// Gets the sale number.
    /// </summary>
    public string Number { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the sale occurred.
    /// </summary>
    public DateTime OccurredAt { get; private set; }

    /// <summary>
    /// Gets the customer identifier.
    /// </summary>
    public Guid CustomerId { get; private set; }

    /// <summary>
    /// Gets the customer name stored for external identity reference.
    /// </summary>
    public string CustomerName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the customer associated with the sale.
    /// </summary>
    public User Customer { get; private set; } = default!;

    /// <summary>
    /// Gets the total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; private set; }

    /// <summary>
    /// Gets the branch identifier where the sale occurred.
    /// </summary>
    public Guid BranchId { get; private set; }

    /// <summary>
    /// Gets the branch name stored for external identity reference.
    /// </summary>
    public string BranchName { get; private set; } = string.Empty;

    /// <summary>
    /// Gets the branch associated with the sale.
    /// </summary>
    public Branch Branch { get; private set; } = default!;

    /// <summary>
    /// Gets a value indicating whether the sale is cancelled.
    /// </summary>
    public bool IsCancelled { get; private set; }

    private readonly List<SaleItem> _items = [];
    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private Sale() { }

    public Sale(
        Guid id,
        string number,
        DateTime occuredAt,
        Guid customerId,
        string customerName,
        Guid branchId,
        string branchName) : base(id)
    {
        Number = number;
        OccurredAt = occuredAt;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        IsCancelled = false;

        Raise(new SaleCreatedEvent(this));
    }

    /// <summary>
    /// Adds an item to the sale.
    /// </summary>
    /// <param name="productId"></param>
    /// <param name="quantity"></param>
    /// <param name="unitPrice"></param>
    /// <exception cref="DomainException"></exception>
    public void AddItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        EnsureCanModify();
        EnsureValidItem(productId, productName, quantity, unitPrice);

        var existingItem = _items.FirstOrDefault(i => !i.IsCancelled && i.ProductId == productId);
        var newQuantity = existingItem is null ? quantity : existingItem.Quantity + quantity;

        if (newQuantity > 20)
        {
            throw new DomainException("Maximum 20 items per product allowed.");
        }

        if (existingItem is not null && existingItem.UnitPrice != unitPrice)
        {
            throw new DomainException("Unit price must be consistent for the same product.");
        }

        if (existingItem is not null && existingItem.ProductName != productName)
        {
            throw new DomainException("Product name must be consistent for the same product.");
        }

        var discount = CalculateDiscount(newQuantity);
        var itemTotal = CalculateItemTotal(newQuantity, unitPrice, discount);

        if (existingItem is null)
        {
            _items.Add(new SaleItem(productId, productName, newQuantity, unitPrice, discount, itemTotal));
        }
        else
        {
            existingItem.UpdatePricing(productName, newQuantity, unitPrice, discount, itemTotal);
        }

        UpdateTotalAmount();
        Raise(new SaleModifiedEvent(this));
    }

    /// <summary>
    /// Replaces the sale items with a new set.
    /// </summary>
    /// <param name="items">The items to be applied.</param>
    public void ReplaceItems(IEnumerable<SaleItemDraft> items)
    {
        EnsureCanModify();

        var normalizedItems = NormalizeItems(items);
        var activeItems = _items.Where(i => !i.IsCancelled).ToDictionary(i => i.ProductId, i => i);

        foreach (var existing in activeItems.Values)
        {
            if (!normalizedItems.Any(item => item.ProductId == existing.ProductId))
            {
                existing.Cancel();
            }
        }

        foreach (var item in normalizedItems)
        {
            var discount = CalculateDiscount(item.Quantity);
            var itemTotal = CalculateItemTotal(item.Quantity, item.UnitPrice, discount);

            if (activeItems.TryGetValue(item.ProductId, out var existing))
            {
                existing.UpdatePricing(item.ProductName, item.Quantity, item.UnitPrice, discount, itemTotal);
            }
            else
            {
                _items.Add(new SaleItem(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice, discount, itemTotal));
            }
        }

        UpdateTotalAmount();
        Raise(new SaleModifiedEvent(this));
    }

    /// <summary>
    /// Removes an item from the sale.
    /// </summary>
    /// <param name="itemId">The sale item identifier.</param>
    /// <exception cref="DomainException"></exception>
    public void CancelItem(Guid itemId)
    {
        EnsureCanModify();

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new DomainException("Item not found in the sale.");

        item.Cancel();

        UpdateTotalAmount();
        Raise(new SaleModifiedEvent(this));
    }

    /// <summary>
    /// Cancels the sale.
    /// </summary>
    public void Cancel()
    {
        if (IsCancelled) return;

        IsCancelled = true;

        foreach (var item in _items.Where(i => !i.IsCancelled))
        {
            item.Cancel();
        }

        UpdateTotalAmount();
        Raise(new SaleCancelledEvent(this));
    }

    /// <summary>
    /// Updates the total amount of the sale.
    /// </summary>
    public void UpdateTotalAmount() =>
        TotalAmount = _items.Where(item => !item.IsCancelled).Sum(item => item.Total);

    /// <summary>
    /// Calculates the discount based on the quantity of items.
    /// </summary>
    /// <param name="quantity"></param>
    /// <returns></returns>
    private static decimal CalculateDiscount(int quantity) =>
        quantity switch
        {
            >= 10 and <= 20 => 0.20m,
            >= 4 and < 10 => 0.10m,
            _ => 0m
        };

    /// <summary>
    /// Calculates the total price of an item based on quantity, unit price, and discount.
    /// </summary>
    /// <param name="quantity"></param>
    /// <param name="unitPrice"></param>
    /// <param name="discount"></param>
    /// <returns></returns>
    private static decimal CalculateItemTotal(int quantity, decimal unitPrice, decimal discount) =>
        quantity * unitPrice * (1 - discount);

    private static void EnsureValidItem(Guid productId, string productName, int quantity, decimal unitPrice)
    {
        if (productId == Guid.Empty)
        {
            throw new DomainException("Product ID is required.");
        }

        if (string.IsNullOrWhiteSpace(productName))
        {
            throw new DomainException("Product name is required.");
        }

        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        if (unitPrice <= 0)
        {
            throw new DomainException("Unit price must be greater than zero.");
        }
    }

    private static IReadOnlyCollection<SaleItemDraft> NormalizeItems(IEnumerable<SaleItemDraft> items)
    {
        if (items is null)
        {
            throw new DomainException("Items are required.");
        }

        var groupedItems = items
            .GroupBy(i => i.ProductId)
            .Select(group =>
            {
                var unitPrices = group.Select(i => i.UnitPrice).Distinct().ToList();
                if (unitPrices.Count > 1)
                {
                    throw new DomainException("Unit price must be consistent for the same product.");
                }

                var productNames = group.Select(i => i.ProductName).Distinct().ToList();
                if (productNames.Count > 1)
                {
                    throw new DomainException("Product name must be consistent for the same product.");
                }

                var totalQuantity = group.Sum(i => i.Quantity);
                var unitPrice = unitPrices.Single();
                var productName = productNames.Single();

                EnsureValidItem(group.Key, productName, totalQuantity, unitPrice);

                if (totalQuantity > 20)
                {
                    throw new DomainException("Maximum 20 items per product allowed.");
                }

                return new SaleItemDraft(group.Key, productName, totalQuantity, unitPrice);
            })
            .ToList();

        if (groupedItems.Count == 0)
        {
            throw new DomainException("At least one item is required.");
        }

        return groupedItems;
    }

    private void EnsureCanModify()
    {
        if (IsCancelled)
        {
            throw new DomainException("Cannot modify cancelled sale.");
        }
    }
}
