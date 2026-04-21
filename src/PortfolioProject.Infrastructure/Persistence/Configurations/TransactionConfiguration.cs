using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioProject.Domain.Entities;

namespace PortfolioProject.Infrastructure.Persistence.Configurations;

public class TransactionConfiguration : BaseEntityConfiguration<Transaction>
{
    public override void Configure(EntityTypeBuilder<Transaction> builder)
    {
        base.Configure(builder);

        builder.ToTable("Transactions");

        builder.Property(t => t.Type).IsRequired().HasConversion<int>();

        builder.Property(t => t.Quantity).HasPrecision(18, 8);
        builder.Property(t => t.Price).HasPrecision(18, 8);
        builder.Property(t => t.SplitRatio).HasPrecision(18, 8);

        builder.Property(t => t.Fees).IsRequired().HasPrecision(18, 4).HasDefaultValue(0m);
        builder.Property(t => t.Amount).IsRequired().HasPrecision(18, 4);

        builder.Property(t => t.Currency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
        builder.Property(t => t.ExecutedAt).IsRequired();

        builder.Property(t => t.Notes).HasMaxLength(1000);
        builder.Property(t => t.ExternalId).HasMaxLength(100);

        builder.HasOne<Portfolio>()
            .WithMany()
            .HasForeignKey(t => t.PortfolioId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Asset>()
            .WithMany()
            .HasForeignKey(t => t.AssetId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => new { t.PortfolioId, t.ExecutedAt });
        builder.HasIndex(t => new { t.AssetId, t.ExecutedAt });
    }
}
