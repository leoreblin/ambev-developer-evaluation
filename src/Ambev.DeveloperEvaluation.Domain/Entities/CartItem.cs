namespace Ambev.DeveloperEvaluation.Domain.Entities;

public record CartItem
{
    public Product Product { get; set; } = new Product();
    public int Quantity { get; set; }
}
