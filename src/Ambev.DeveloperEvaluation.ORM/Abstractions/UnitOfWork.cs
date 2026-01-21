using Ambev.DeveloperEvaluation.Application.Abstractions;
using Ambev.DeveloperEvaluation.Domain.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.ORM.Abstractions;

/// <summary>
/// Represents the unit of work concrete class.
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private readonly DefaultContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private readonly IPublisher _publisher;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
    /// </summary>
    /// <param name="context">The DB context.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="publisher">The MediatR publisher.</param>
    public UnitOfWork(DefaultContext context, ILogger<UnitOfWork> logger, IPublisher publisher)
    {
        _context = context;
        _logger = logger;
        _publisher = publisher;
    }

    /// <inheritdoc />
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var aggregates = _context.ChangeTracker
            .Entries<AggregateRoot>()
            .Where(entry => entry.Entity.Events.Count != 0)
            .Select(entry => entry.Entity)
            .ToList();

        var domainEvents = aggregates
            .SelectMany(aggregate => aggregate.Events)
            .ToList();

        aggregates.ForEach(aggregate => aggregate.ClearEvents());

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, cancellationToken);
        }
    }
}
