using Ambev.DeveloperEvaluation.Data.NoSql.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Models;

[BsonCollection("products")]
public class ProductDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [BsonElement("external_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid ExternalId { get; set; }

    [BsonElement("title")]
    public string Title { get; set; } = string.Empty;

    [BsonElement("price")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Price { get; set; }

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("category")]
    public string Category { get; set; } = string.Empty;

    [BsonElement("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [BsonElement("rating")]
    public ProductRating Rating { get; set; } = new ProductRating();
}
