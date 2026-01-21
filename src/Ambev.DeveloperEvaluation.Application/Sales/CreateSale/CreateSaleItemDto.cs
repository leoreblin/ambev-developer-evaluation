namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public readonly record struct CreateSaleItemDto
{
    public required Guid ProductId { get; init; }
    public required int Quantity { get; init; }
    public required decimal UnitPrice { get; init; }
}
