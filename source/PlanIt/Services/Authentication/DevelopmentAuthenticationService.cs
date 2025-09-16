using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PlanIt.Services.Interfaces;
using IAuthenticationService = PlanIt.Services.Interfaces.IAuthenticationService;

namespace PlanIt.Services.Authentication
{
    /// <summary>
    /// Development authentication service that automatically authenticates users
    /// </summary>
    public class DevelopmentAuthenticationService : IAuthenticationService
    {
        private readonly ILogger<DevelopmentAuthenticationService> _logger;

        public DevelopmentAuthenticationService(ILogger<DevelopmentAuthenticationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string PrepareSignIn(string username)
        {
            // In development mode, we don't need to prepare sign-in
            // Just return a dummy session ID
            _logger.LogDebug("Development mode: Skipping sign-in preparation for {Username}", username);
            return "dev-session-" + Guid.NewGuid().ToString("N");
        }

        public string? GetPreparedSignIn(string sessionId)
        {
            // In development mode, always return the development user
            if (sessionId.StartsWith("dev-session-"))
            {
                _logger.LogDebug("Development mode: Returning development user for session {SessionId}", sessionId);
                return "dev@example.com";
            }

            return null;
        }
        

        /// <summary>
        /// Automatically authenticicate a user in development mode
        /// </summary>
        public async Task<string> AutoAuthenticateAsync(HttpContext httpContext)
        {
            var username = "dev@example.com";
            var displayName = "Development User";

            _logger.LogInformation("Development mode: Auto-authenticating user {Username}", username);

            var claims = CreateUserClaims(username, displayName);
            await SignInUserAsync(httpContext, claims);

            return username;
        }

        public ClaimsPrincipal CreateUserClaims(string username, string displayName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Email, username),
                new Claim(ClaimTypes.NameIdentifier, username.Split('@')[0]),
                new Claim(ClaimTypes.Role, "ConnectedUser")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            return new ClaimsPrincipal(identity);
        }

        public async Task SignInUserAsync(HttpContext httpContext, ClaimsPrincipal claims)
        {
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims);
        }
        
    }
} 