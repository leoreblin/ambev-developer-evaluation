using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

public sealed record UpdateSaleRequest
{
    public required List<UpdateSaleItemDto> Items { get; init; }
}
