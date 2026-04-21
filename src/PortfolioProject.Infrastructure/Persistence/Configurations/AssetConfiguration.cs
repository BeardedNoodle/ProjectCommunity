using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioProject.Domain.Entities;

namespace PortfolioProject.Infrastructure.Persistence.Configurations;

public class AssetConfiguration : BaseEntityConfiguration<Asset>
{
    public override void Configure(EntityTypeBuilder<Asset> builder)
    {
        base.Configure(builder);

        builder.ToTable("Assets");

        builder.Property(a => a.Symbol).IsRequired().HasMaxLength(20);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.AssetClass).IsRequired().HasConversion<int>();
        builder.Property(a => a.Exchange).IsRequired().HasMaxLength(50);
        builder.Property(a => a.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(a => a.IsActive).IsRequired().HasDefaultValue(true);
        builder.Property(a => a.IsFractionable).IsRequired().HasDefaultValue(false);
        builder.Property(a => a.Sector).HasMaxLength(100);
        builder.Property(a => a.ExternalId).HasMaxLength(100);

        builder.HasIndex(a => new { a.Symbol, a.AssetClass }).IsUnique();
    }
}
