namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public sealed record CreateSaleFromCartRequest
{
    public required Guid CustomerId { get; set; }
    public required Guid BranchId { get; set; }
}
