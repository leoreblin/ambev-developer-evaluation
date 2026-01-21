using System.Reflection;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM;

public class DefaultContext : DbContext
{
    private readonly string _fallbackConnString = "Host=ambev.developerevaluation.database;Port=5432;Database=developer_evaluation;Username=developer;Password=ev@luAt10n";

    public DbSet<User> Users { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Branch> Branches { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultContext"/> class.
    /// </summary>
    /// <remarks>Used by EF Core Tools.</remarks>
    public DefaultContext() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultContext"/> class with the specified options.
    /// </summary>
    /// <param name="options">The specified options.</param>
    public DefaultContext(DbContextOptions<DefaultContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(_fallbackConnString);
        }

        base.OnConfiguring(optionsBuilder);
    }
}