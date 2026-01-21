using Ambev.DeveloperEvaluation.Domain.Events;
using DnsClient.Internal;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

internal sealed class SaleCancelledEventHandler : INotificationHandler<SaleCancelledEvent>
{
    private readonly ILogger<SaleCancelledEventHandler> _logger;

    public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("""
            Sale cancelled: {SaleId}
            Cusomer ID: {CustomerId}
            Branch ID: {BranchId}
            """,
            notification.Sale.Id, notification.Sale.CustomerId, notification.Sale.BranchId);

        await Task.CompletedTask;
    }
}
