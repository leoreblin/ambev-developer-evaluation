using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Services;

namespace Ambev.DeveloperEvaluation.Functional;

public sealed class FakeCartService : ICartService
{
    private readonly Dictionary<Guid, Cart> _carts = new();

    public Task AddToCartAsync(Guid userId, Guid productId, int quantity, CancellationToken cancellationToken = default)
    {
        if (!_carts.TryGetValue(userId, out var cart))
        {
            cart = new Cart { UserId = userId.ToString() };
            _carts[userId] = cart;
        }

        cart.Items.Add(new CartItem
        {
            Product = new Product { Id = productId, Price = 10m, Title = "Test Product" },
            Quantity = quantity
        });

        return Task.CompletedTask;
    }

    public Task<Cart> GetCartAsync(Guid userId, CancellationToken cancellationToken)
        => Task.FromResult(_carts.TryGetValue(userId, out var cart) ? cart : null!);

    public Task ClearCartAsync(Guid userId)
    {
        _carts.Remove(userId);
        return Task.CompletedTask;
    }

    public Task RemoveProductAsync(Guid userId, Guid productId)
    {
        if (_carts.TryGetValue(userId, out var cart))
        {
            cart.Items.RemoveAll(item => item.Product.Id == productId);
        }

        return Task.CompletedTask;
    }
}
