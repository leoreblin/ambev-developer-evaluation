using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

public record GetSaleResponse
{
    public Guid Id { get; set; }
    public string Number { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
    public IEnumerable<GetSaleItemResponse> Items { get; set; } = [];

    public static implicit operator GetSaleResponse(Sale entity)
    {
        return new()
        {
            Id = entity.Id,
            Number = entity.Number,
            OccurredAt = entity.OccurredAt,
            CustomerId = entity.CustomerId,
            BranchId = entity.BranchId,
            TotalAmount = Math.Round(entity.TotalAmount, 2),
            CustomerName = entity.Customer.Username,
            BranchName = entity.Branch.Name,
            IsCancelled = entity.IsCancelled,
            Items = entity.Items.Select(i => (GetSaleItemResponse)i)
        };
    }
}
