using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

public sealed record CreateSaleRequest
{
    public required Guid CustomerId { get; set; }

    public required Guid BranchId { get; set; }

    public required List<CreateSaleItemDto> Items { get; set; }
}
