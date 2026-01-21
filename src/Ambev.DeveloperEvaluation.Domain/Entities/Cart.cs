namespace Ambev.DeveloperEvaluation.Domain.Entities;

public record Cart
{
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public List<CartItem> Items { get; set; } = [];
    public decimal Total => Items.Sum(i => i.Product.Price * i.Quantity);
}