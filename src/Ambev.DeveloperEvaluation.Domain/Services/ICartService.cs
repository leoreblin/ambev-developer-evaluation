using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Services;

public interface ICartService
{
    /// <summary>
    /// Adds a product to the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="productId">The product identifier.</param>
    /// <param name="quantity">The products quantity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    Task AddToCartAsync(Guid userId, Guid productId, int quantity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A result task with the <see cref="Cart"/>.</returns>
    Task<Cart> GetCartAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a product from the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A completed <see cref="Task"/>.</returns>
    Task ClearCartAsync(Guid userId);

    /// <summary>
    /// Removes a product from the user's cart.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="productId">The product identifier.</param>
    /// <returns></returns>
    Task RemoveProductAsync(Guid userId, Guid productId);
}
