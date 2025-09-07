using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PlanIt.Services.Interfaces;

namespace PlanIt.Services.Authentication
{
    /// <summary>
    /// Handles user session operations including claims creation and authentication
    /// </summary>
    public class UserSessionService : IUserSessionService
    {
        private readonly ILogger<UserSessionService> _logger;

        public UserSessionService(ILogger<UserSessionService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ClaimsPrincipal CreateUserClaims(string username, string displayName)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            if (string.IsNullOrWhiteSpace(displayName))
                throw new ArgumentException("Display name cannot be null or empty", nameof(displayName));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Email, username),
                new Claim(ClaimTypes.NameIdentifier, username.Split('@')[0]),
                new Claim(ClaimTypes.Role, "ConnectedUser")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            _logger.LogDebug("Created user claims for {Username} with display name {DisplayName}", username, displayName);
            return principal;
        }

        public async Task SignInUserAsync(HttpContext httpContext, ClaimsPrincipal claims)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            if (claims == null)
                throw new ArgumentNullException(nameof(claims));

            await httpContext.SignInAsync(claims);
            
            var username = claims.FindFirst(ClaimTypes.Email)?.Value ?? "Unknown";
            _logger.LogInformation("User {Username} signed in successfully", username);
        }

        public async Task SignOutUserAsync(HttpContext httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            await httpContext.SignOutAsync();
            _logger.LogInformation("User signed out successfully");
        }
    }
} 