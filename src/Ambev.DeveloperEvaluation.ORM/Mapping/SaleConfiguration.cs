using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.Number).IsRequired().HasMaxLength(100);
        builder.Property(s => s.OccurredAt).IsRequired().HasDefaultValue(DateTime.UtcNow);

        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Branch)
            .WithMany()
            .HasForeignKey(s => s.BranchId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.OwnsMany(s => s.Items, itemBuilder =>
        {
            itemBuilder.ToTable("SaleItems");
            itemBuilder.WithOwner().HasForeignKey("SaleId");
            itemBuilder.Property(i => i.ProductId).HasColumnType("uuid").IsRequired();
            itemBuilder.Property(i => i.Quantity).IsRequired();
            itemBuilder.Property(i => i.UnitPrice).IsRequired().HasPrecision(18, 2);
            itemBuilder.Property(i => i.Discount).IsRequired().HasPrecision(18, 2);
            itemBuilder.Property(i => i.Total).IsRequired().HasPrecision(18, 2);
            itemBuilder.Property(i => i.IsCancelled).IsRequired().HasDefaultValue(false);
        });
    }
}
