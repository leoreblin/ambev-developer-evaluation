using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Branch : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public string FederalId { get; private set; } = string.Empty;

    /// <summary>
    /// Required by EF Core.
    /// </summary>
    private Branch() { }

    public Branch(string name, string federalId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("Invalid name of Branch.");
        }

        if (string.IsNullOrWhiteSpace(federalId) || federalId.Length != 14)
        {
            throw new DomainException("Invalid Federal ID. It must contain 14 digits only.");
        }

        Name = name;
        FederalId = federalId;
    }
}
