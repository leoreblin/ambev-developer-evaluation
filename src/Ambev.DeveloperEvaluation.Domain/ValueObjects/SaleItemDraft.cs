namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public sealed record SaleItemDraft(Guid ProductId, int Quantity, decimal UnitPrice);
