using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.ModifySale;

internal sealed class SaleModifiedEventHandler : INotificationHandler<SaleModifiedEvent>
{
    private readonly ILogger<SaleModifiedEventHandler> _logger;

    public SaleModifiedEventHandler(ILogger<SaleModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            """
            Sale modified: {SaleId}
            Customer ID: {CustomerId}
            Branch ID: {BranchId}
            """,
            notification.Sale.Id, notification.Sale.CustomerId, notification.Sale.BranchId);

        await Task.CompletedTask;
    }
}
