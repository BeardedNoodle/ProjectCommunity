using PortfolioProject.Domain.Common;

namespace PortfolioProject.Domain.Entities;

public class Portfolio : BaseEntity
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public bool IsDefault { get; set; }
}
