using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public sealed class BranchRepository : IBranchRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="BranchRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BranchRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Branch>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Branches.ToListAsync(cancellationToken);
    }
}
