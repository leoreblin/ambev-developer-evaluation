namespace Ambev.DeveloperEvaluation.Domain.Common;

public abstract class AggregateRoot : BaseEntity
{
    private readonly List<IEvent> _events = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class with the specified ID.
    /// </summary>
    /// <param name="id">The aggregate root identifier.</param>
    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
    /// </summary>
    /// <remarks>Required by EF Core.</remarks>
    protected AggregateRoot()
    {
    }

    /// <summary>
    /// Gets the events that have been raised by this aggregate root.
    /// </summary>
    /// <returns>The readonly collection of events.</returns>
    public IReadOnlyCollection<IEvent> Events => [.. _events];

    /// <summary>
    /// Clears the events.
    /// </summary>
    public void ClearEvents() => _events.Clear();

    /// <summary>
    /// Raises the specified event.
    /// </summary>
    /// <param name="event">The event.</param>
    protected void Raise(IEvent @event) => _events.Add(@event);
}
