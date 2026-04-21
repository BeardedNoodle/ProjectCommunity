using PortfolioProject.Domain.Common;

namespace PortfolioProject.Domain.Entities;

public class User : BaseEntity
{
    public string ExternalId { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string? PhoneNumber { get; set; }
    public string? TelegramChatId { get; set; }
    public string? DiscordUserId { get; set; }

    public string TimeZone { get; set; } = "UTC";
    public string PreferredCurrency { get; set; } = "USD";

    public DateTime? LastLoginAt { get; set; }
}
