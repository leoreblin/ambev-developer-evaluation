using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

public interface IBranchRepository
{
    /// <summary>
    /// Get all branches.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A collection of branches.</returns>
    Task<IEnumerable<Branch>> GetAllAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a branch by its identifier.
    /// </summary>
    /// <param name="branchId">The branch identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The branch if found.</returns>
    Task<Branch?> GetByIdAsync(
        Guid branchId,
        CancellationToken cancellationToken = default);
}
