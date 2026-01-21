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
}
