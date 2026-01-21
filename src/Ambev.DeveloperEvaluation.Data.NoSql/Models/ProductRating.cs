using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Models;

public class ProductRating
{
    [BsonElement("rate")]
    [BsonRepresentation(BsonType.Decimal128)]
    public decimal Rate { get; set; }

    [BsonElement("count")]
    public int Count { get; set; }
}
