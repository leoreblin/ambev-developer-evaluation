using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public record CancelSaleItemCommand(Guid SaleId, Guid SaleItemId) : IRequest<Unit>;
