using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioProject.Domain.Entities;

namespace PortfolioProject.Infrastructure.Persistence.Configurations;

public class PriceConfiguration : BaseEntityConfiguration<Price>
{
    public override void Configure(EntityTypeBuilder<Price> builder)
    {
        base.Configure(builder);

        builder.ToTable("Prices");

        builder.Property(p => p.Timestamp).IsRequired();

        builder.Property(p => p.Open).HasPrecision(18, 8);
        builder.Property(p => p.High).HasPrecision(18, 8);
        builder.Property(p => p.Low).HasPrecision(18, 8);
        builder.Property(p => p.Close).IsRequired().HasPrecision(18, 8);

        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(p => p.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.AssetId, p.Timestamp }).IsUnique();
        builder.HasIndex(p => p.Timestamp);
    }
}
