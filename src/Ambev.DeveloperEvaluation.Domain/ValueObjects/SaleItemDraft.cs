namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record SaleItemDraft(Guid ProductId, string ProductName, int Quantity, decimal UnitPrice);
