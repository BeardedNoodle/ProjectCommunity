using Microsoft.Extensions.Hosting;
using PortfolioProject.Infrastructure.Persistence;

namespace PortfolioProject.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<ApplicationDbContext>("portfoliodb");

        return builder;
    }
}
