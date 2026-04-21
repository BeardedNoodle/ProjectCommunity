using Microsoft.EntityFrameworkCore;
using PortfolioProject.Domain.Entities;
using PortfolioProject.Infrastructure.Persistence.Interceptors;

namespace PortfolioProject.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    AuditSaveChangesInterceptor? auditInterceptor = null) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Portfolio> Portfolios => Set<Portfolio>();
    public DbSet<Asset> Assets => Set<Asset>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<Price> Prices => Set<Price>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // auditInterceptor is null at design-time (migrations) where DI isn't available.
        if (auditInterceptor is not null)
        {
            optionsBuilder.AddInterceptors(auditInterceptor);
        }

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
