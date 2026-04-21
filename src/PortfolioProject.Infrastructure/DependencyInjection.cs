using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PortfolioProject.Application.Common.Interfaces;
using PortfolioProject.Infrastructure.Identity;
using PortfolioProject.Infrastructure.Persistence;
using PortfolioProject.Infrastructure.Persistence.Interceptors;

namespace PortfolioProject.Infrastructure;

public static class DependencyInjection
{
    public static IHostApplicationBuilder AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
        builder.Services.AddScoped<AuditSaveChangesInterceptor>();

        builder.AddNpgsqlDbContext<ApplicationDbContext>("portfoliodb");

        return builder;
    }
}
