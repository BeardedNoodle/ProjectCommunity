using PortfolioProject.Application.Common.Interfaces;

namespace PortfolioProject.Infrastructure.Identity;

/// <summary>
/// Stub implementation. Replace with a Keycloak-aware version that reads the
/// local User.Id (mapped from the JWT <c>sub</c> claim) once auth is wired up.
/// </summary>
internal sealed class CurrentUser : ICurrentUser
{
    public Guid? UserId => null;
}
