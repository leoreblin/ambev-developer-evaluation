using Ambev.DeveloperEvaluation.Data.NoSql.Models;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Extensions;

public static class ProductMappingExtensions
{
    public static Product ToDomain(this ProductDocument document) =>
        new()
        {
            Id = document.ExternalId,
            Title = document.Title,
            Price = document.Price,
            Description = document.Description,
            Category = document.Category,
            ImageUrl = document.ImageUrl,
            Rating = new Domain.Entities.ProductRating
            {
                Rate = document.Rating.Rate,
                Count = document.Rating.Count
            }
        };    
}
