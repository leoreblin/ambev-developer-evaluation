namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public sealed record UpdateSaleItemDto
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
}
