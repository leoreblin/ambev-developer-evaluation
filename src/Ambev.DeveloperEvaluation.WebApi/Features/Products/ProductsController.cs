using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProducts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products;

/// <summary>
/// Represents the products controller.
/// </summary>
[ApiController]
[Route("products")]
public class ProductsController : BaseController
{
    private readonly IProductRepository _productRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductsController"/> class.
    /// </summary>
    /// <param name="productRepo"></param>
    public ProductsController(IProductRepository productRepo)
    {
        _productRepository = productRepo;
    }

    /// <summary>
    /// Get a paginated list of products.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="orderBy">The sorting field.</param>
    /// <param name="isDescending">The sorting direction.</param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponseWithData<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Get([FromQuery] GetProductsQueryData query)
    {
        if (!string.IsNullOrWhiteSpace(query.OrderBy) &&
            !PropertyHelper<Product>.IsValidProperty(query.OrderBy))
        {
            throw new ValidationException(
                $"Invalid sort property '{query.OrderBy}'. Valid properties are: " +
                string.Join(", ", PropertyHelper<Product>.GetValidProperties()));
        }

        var products = await _productRepository.GetPaginatedAsync(
            query.PageNumber,
            query.PageSize,
            query.OrderBy, 
            query.IsDescending,
            query.Term);

        return OkPaginated(products);
    }
}
