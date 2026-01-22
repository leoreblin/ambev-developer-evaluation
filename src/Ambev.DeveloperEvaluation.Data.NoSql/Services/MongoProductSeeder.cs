using System.Globalization;
using System.Text.Json;
using Ambev.DeveloperEvaluation.Data.NoSql.Context;
using Ambev.DeveloperEvaluation.Data.NoSql.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Ambev.DeveloperEvaluation.Data.NoSql.Services;

public static class MongoProductSeeder
{
    private const int MinSeedCount = 100;

    public static async Task SeedIfEmptyAsync(
        MongoDbContext context,
        string contentRootPath,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentException.ThrowIfNullOrWhiteSpace(contentRootPath);
        ArgumentNullException.ThrowIfNull(logger);

        var existingCount = await context.Products.CountDocumentsAsync(
            Builders<ProductDocument>.Filter.Empty,
            cancellationToken: cancellationToken);

        if (existingCount > 0)
        {
            logger.LogInformation("MongoDB products already seeded.");
            return;
        }

        var dumpPath = FindDumpPath(contentRootPath);
        if (string.IsNullOrWhiteSpace(dumpPath))
        {
            logger.LogWarning("Products dump file not found. MongoDB seeding skipped.");
            return;
        }

        logger.LogInformation("Using products dump at {DumpPath}.", dumpPath);

        var products = LoadProductsFromDump(dumpPath);
        if (products.Count == 0)
        {
            logger.LogWarning("No products found in dump file. MongoDB seeding skipped.");
            return;
        }

        if (products.Count < MinSeedCount)
        {
            logger.LogWarning(
                "Products dump contains only {Count} items. Seeding anyway.",
                products.Count);
        }

        await context.Products.InsertManyAsync(products, cancellationToken: cancellationToken);
        logger.LogInformation("Seeded {Count} products into MongoDB.", products.Count);
    }

    private static string? FindDumpPath(string contentRootPath)
    {
        var candidates = new[]
        {
            Path.Combine(contentRootPath, "products-dump.json"),
            Path.GetFullPath(Path.Combine(contentRootPath, "..", ".doc", "products-dump.json"))
        };

        return candidates.FirstOrDefault(File.Exists);
    }

    private static List<ProductDocument> LoadProductsFromDump(string path)
    {
        using var stream = File.OpenRead(path);
        using var document = JsonDocument.Parse(stream);

        if (document.RootElement.ValueKind != JsonValueKind.Array)
        {
            return [];
        }

        var products = new List<ProductDocument>();
        foreach (var element in document.RootElement.EnumerateArray())
        {
            var product = new ProductDocument
            {
                Id = GetString(element, "_id", "$oid") ?? string.Empty,
                ExternalId = GetGuid(element, "external_id"),
                Title = GetString(element, "title") ?? string.Empty,
                Price = GetDecimal(element, "price", "$numberDecimal"),
                Description = GetString(element, "description") ?? string.Empty,
                Category = GetString(element, "category") ?? string.Empty,
                ImageUrl = GetString(element, "image_url") ?? string.Empty,
                Rating = new ProductRating
                {
                    Rate = GetDecimal(element, "rating", "rate"),
                    Count = GetInt(element, "rating", "count")
                }
            };

            products.Add(product);
        }

        return products;
    }

    private static string? GetString(JsonElement element, string propertyName, string? nestedProperty = null)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            return null;
        }

        if (nestedProperty is not null && value.ValueKind == JsonValueKind.Object &&
            value.TryGetProperty(nestedProperty, out var nestedValue))
        {
            return nestedValue.GetString();
        }

        return value.ValueKind == JsonValueKind.String ? value.GetString() : null;
    }

    private static Guid GetGuid(JsonElement element, string propertyName)
    {
        var value = GetString(element, propertyName);
        return Guid.TryParse(value, out var guid) ? guid : Guid.Empty;
    }

    private static decimal GetDecimal(JsonElement element, string propertyName, string nestedProperty)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            return 0m;
        }

        if (value.ValueKind == JsonValueKind.Object &&
            value.TryGetProperty(nestedProperty, out var nestedValue))
        {
            return ParseDecimal(nestedValue);
        }

        return ParseDecimal(value);
    }

    private static decimal ParseDecimal(JsonElement value)
    {
        if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var number))
        {
            return number;
        }

        if (value.ValueKind == JsonValueKind.String &&
            decimal.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var parsed))
        {
            return parsed;
        }

        return 0m;
    }

    private static int GetInt(JsonElement element, string propertyName, string nestedProperty)
    {
        if (!element.TryGetProperty(propertyName, out var value))
        {
            return 0;
        }

        if (value.ValueKind == JsonValueKind.Object &&
            value.TryGetProperty(nestedProperty, out var nestedValue) &&
            nestedValue.ValueKind == JsonValueKind.Number &&
            nestedValue.TryGetInt32(out var number))
        {
            return number;
        }

        return 0;
    }
}
