using Ambev.DeveloperEvaluation.Data.NoSql.Configurations;
using Ambev.DeveloperEvaluation.Data.NoSql.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Context;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(
        IOptions<MongoDbSettings> settings,
        IMongoClient client)
    {
        _settings = settings.Value;
        _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<ProductDocument> Products =>
        _database.GetCollection<ProductDocument>(_settings.ProductsCollection);
}
