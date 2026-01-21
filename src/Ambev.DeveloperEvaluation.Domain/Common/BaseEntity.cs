using Ambev.DeveloperEvaluation.Common.Validation;

namespace Ambev.DeveloperEvaluation.Domain.Common;

public abstract class BaseEntity : IComparable<BaseEntity>
{
    public Guid Id { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class with the specified ID.
    /// </summary>
    /// <param name="id">The entity identifier.</param>
    /// <exception cref="ArgumentException">Throws when the ID is invalid.</exception>
    protected BaseEntity(Guid id)
        : this()
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("The entity ID is required", nameof(id));
        }

        Id = id;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseEntity"/> class.
    /// </summary>
    /// <remarks>Required for deserialization.</remarks>
    protected BaseEntity()
    {
    }

    public Task<IEnumerable<ValidationErrorDetail>> ValidateAsync()
    {
        return Validator.ValidateAsync(this);
    }

    public int CompareTo(BaseEntity? other)
    {
        if (other == null)
        {
            return 1;
        }

        return other!.Id.CompareTo(Id);
    }
}
