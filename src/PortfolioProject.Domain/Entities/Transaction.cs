using PortfolioProject.Domain.Common;
using PortfolioProject.Domain.Enums;

namespace PortfolioProject.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid PortfolioId { get; set; }
    public Guid? AssetId { get; set; }

    public TransactionType Type { get; set; }

    public decimal? Quantity { get; set; }
    public decimal? Price { get; set; }

    public decimal Fees { get; set; }
    public decimal Amount { get; set; }

    public string Currency { get; set; } = "USD";

    public DateTime ExecutedAt { get; set; }

    public decimal? SplitRatio { get; set; }

    public string? Notes { get; set; }
    public string? ExternalId { get; set; }
}
