using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioProject.Domain.Entities;

namespace PortfolioProject.Infrastructure.Persistence.Configurations;

public class UserConfiguration : BaseEntityConfiguration<User>
{
    public override void Configure(EntityTypeBuilder<User> builder)
    {
        base.Configure(builder);

        builder.ToTable("Users");

        builder.Property(u => u.ExternalId).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.ExternalId).IsUnique();

        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);

        builder.Property(u => u.PhoneNumber).HasMaxLength(32);
        builder.Property(u => u.TelegramChatId).HasMaxLength(64);
        builder.Property(u => u.DiscordUserId).HasMaxLength(64);

        builder.Property(u => u.TimeZone).IsRequired().HasMaxLength(64).HasDefaultValue("UTC");
        builder.Property(u => u.PreferredCurrency).IsRequired().HasMaxLength(3).HasDefaultValue("USD");
    }
}
