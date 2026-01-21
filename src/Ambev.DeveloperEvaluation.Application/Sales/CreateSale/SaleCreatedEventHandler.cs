using Ambev.DeveloperEvaluation.Domain.Events;
using DnsClient.Internal;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

internal sealed class SaleCreatedEventHandler : INotificationHandler<SaleCreatedEvent>
{
    private readonly ILogger<SaleCreatedEventHandler> _logger;

    public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("""
            Sale created: {SaleId}
            Cusomer ID: {CustomerId}
            Branch ID: {BranchId}
            Total: {TotalAmount}
            """,
            notification.Sale.Id, notification.Sale.CustomerId, notification.Sale.BranchId, notification.Sale.TotalAmount);

        await Task.CompletedTask;
    }
}
