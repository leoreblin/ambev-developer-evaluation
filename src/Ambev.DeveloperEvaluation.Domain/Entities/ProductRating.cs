namespace Ambev.DeveloperEvaluation.Domain.Entities;

public record ProductRating
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
}
