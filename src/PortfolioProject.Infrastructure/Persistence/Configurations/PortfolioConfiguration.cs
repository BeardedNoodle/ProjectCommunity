using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioProject.Domain.Entities;

namespace PortfolioProject.Infrastructure.Persistence.Configurations;

public class PortfolioConfiguration : BaseEntityConfiguration<Portfolio>
{
    public override void Configure(EntityTypeBuilder<Portfolio> builder)
    {
        base.Configure(builder);

        builder.ToTable("Portfolios");

        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.IsDefault).IsRequired().HasDefaultValue(false);

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => new { p.UserId, p.Name }).IsUnique();

        // At most one default portfolio per user (partial unique index).
        // Filter by IsDeleted so a soft-deleted default doesn't block setting a new one.
        builder.HasIndex(p => p.UserId)
            .IsUnique()
            .HasFilter("\"IsDefault\" = true AND \"IsDeleted\" = false")
            .HasDatabaseName("IX_Portfolios_UserId_OneDefault");
    }
}
