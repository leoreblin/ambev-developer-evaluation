using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;

public record GetSaleItemResponse
{
    public Guid ItemId { get; set; }
    public Guid ProductId { get; set; }
    public int ItemQuantity { get; set; }
    public decimal ItemUnitPrice { get; set; }
    public decimal ItemDiscount { get; set; }
    public decimal ItemTotal { get; set; }
    public bool IsCancelled { get; set; }

    public static implicit operator GetSaleItemResponse(SaleItem entity)
    {
        return new()
        {
            ItemId = entity.Id,
            ProductId = entity.ProductId,
            ItemQuantity = entity.Quantity,
            ItemUnitPrice = entity.UnitPrice,
            ItemDiscount = entity.Discount,
            ItemTotal = Math.Round(entity.Total, 2),
            IsCancelled = entity.IsCancelled
        };
    }
}
