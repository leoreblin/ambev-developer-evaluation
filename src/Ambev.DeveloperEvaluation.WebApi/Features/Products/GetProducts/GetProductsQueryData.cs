namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProducts;

public sealed record GetProductsQueryData
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string OrderBy { get; set; } = "Title";
    public bool IsDescending { get; set; } = false;
    public string Term { get; set; } = string.Empty;
}
