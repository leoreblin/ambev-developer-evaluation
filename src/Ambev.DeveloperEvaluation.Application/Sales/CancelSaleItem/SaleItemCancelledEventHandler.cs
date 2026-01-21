using Ambev.DeveloperEvaluation.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

/// <summary>
/// Handles the event when a sale item is cancelled.
/// </summary>
internal sealed class SaleItemCancelledEventHandler : INotificationHandler<SaleItemCancelledEvent>
{
    private readonly ILogger<SaleItemCancelledEventHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaleItemCancelledEventHandler"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public SaleItemCancelledEventHandler(ILogger<SaleItemCancelledEventHandler> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task Handle(SaleItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("""
            Sale item cancelled.
            Sale item ID: {SaleItemId}
            """, notification.Item.Id);

        await Task.CompletedTask;
    }
}
