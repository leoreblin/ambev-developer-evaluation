using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public sealed record UpdateSaleCommand(Guid SaleId, List<UpdateSaleItemDto> Items) : IRequest<Unit>;
