using PortfolioProject.Domain.Common;
using PortfolioProject.Domain.Enums;

namespace PortfolioProject.Domain.Entities;

public class Asset : BaseEntity
{
    public string Symbol { get; set; } = null!;
    public string Name { get; set; } = null!;

    public AssetClass AssetClass { get; set; }
    public string Exchange { get; set; } = null!;
    public string Currency { get; set; } = "USD";

    public bool IsActive { get; set; } = true;
    public bool IsFractionable { get; set; }

    public string? Sector { get; set; }

    public string? ExternalId { get; set; }
}
