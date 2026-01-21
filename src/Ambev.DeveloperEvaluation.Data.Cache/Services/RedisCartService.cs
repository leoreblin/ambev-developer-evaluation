using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Services;
using FluentValidation;
using StackExchange.Redis;

namespace Ambev.DeveloperEvaluation.Data.Cache.Services;

public sealed class RedisCartService : ICartService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IProductRepository _productRepository;
    private IDatabase RedisDb => _redis.GetDatabase();

    public RedisCartService(
        IConnectionMultiplexer redis,
        IProductRepository productRepository)
    {
        _redis = redis;
        _productRepository = productRepository;
    }

    /// <inheritdoc />
    public async Task AddToCartAsync(
        Guid userId,
        Guid productId, 
        int quantity,
        CancellationToken cancellationToken = default)
    {
        _ = await _productRepository.GetByIdAsync(productId, cancellationToken)
            ?? throw new ValidationException($"Product with ID {productId} not found.");

        var cartKey = GetCartKey(userId);
        await RedisDb.HashSetAsync(cartKey, productId.ToString(), quantity);
        await SetCartExpirationAsync(cartKey);
    }

    /// <inheritdoc />
    public async Task<Cart> GetCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cartKey = GetCartKey(userId);
        var entries = await RedisDb.HashGetAllAsync(cartKey);

        var productIds = entries.Select(e => Guid.Parse(e.Name!)).ToList();
        var products = await _productRepository.GetByIdsAsync(productIds, cancellationToken);

        var cart = new Cart { UserId = userId.ToString() };
        foreach (var entry in entries)
        {
            var productId = Guid.Parse(entry.Name!);
            var product = products.FirstOrDefault(p => p.Id == productId);

            if (product != null)
            {
                cart.Items.Add(new CartItem
                {
                    Product = product,
                    Quantity = (int)entry.Value
                });
            }
        }

        return cart;
    }

    /// <inheritdoc />
    public async Task ClearCartAsync(Guid userId)
    {
        var cartKey = GetCartKey(userId);
        await RedisDb.KeyDeleteAsync(cartKey);
    }

    /// <inheritdoc />
    public async Task RemoveProductAsync(Guid userId, Guid productId)
    {
        var cartKey = GetCartKey(userId);
        await RedisDb.HashDeleteAsync(cartKey, productId.ToString());
    }

    private static string GetCartKey(Guid userId) => $"cart:{userId}";

    private async Task SetCartExpirationAsync(string cartKey)
    {
        var expirationTime = TimeSpan.FromHours(1);
        await RedisDb.KeyExpireAsync(cartKey, expirationTime);
    }
}
