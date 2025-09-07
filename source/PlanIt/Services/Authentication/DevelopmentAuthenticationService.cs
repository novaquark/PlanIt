using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using PlanIt.Services.Interfaces;

namespace PlanIt.Services.Authentication
{
    /// <summary>
    /// Development authentication service that automatically authenticates users
    /// </summary>
    public class DevelopmentAuthenticationService : IAppAuthenticationService
    {
        private readonly IDevelopmentModeService _developmentModeService;
        private readonly IUserSessionService _userSessionService;
        private readonly ILogger<DevelopmentAuthenticationService> _logger;

        public DevelopmentAuthenticationService(
            IDevelopmentModeService developmentModeService,
            IUserSessionService userSessionService,
            ILogger<DevelopmentAuthenticationService> logger)
        {
            _developmentModeService = developmentModeService ?? throw new ArgumentNullException(nameof(developmentModeService));
            _userSessionService = userSessionService ?? throw new ArgumentNullException(nameof(userSessionService));
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
                return _developmentModeService.DevelopmentUserEmail;
            }

            return null;
        }

        public void CleanupExpiredSessions()
        {
            // No cleanup needed in development mode
            _logger.LogDebug("Development mode: No session cleanup required");
        }

        /// <summary>
        /// Automatically authenticicates a user in development mode
        /// </summary>
        public async Task<string> AutoAuthenticateAsync(HttpContext httpContext)
        {
            if (!_developmentModeService.BypassAuthentication)
            {
                throw new InvalidOperationException("Auto-authentication is only available when bypass authentication is enabled");
            }

            var username = _developmentModeService.DevelopmentUserEmail;
            var displayName = _developmentModeService.DevelopmentUserName;

            _logger.LogInformation("Development mode: Auto-authenticating user {Username}", username);

            var claims = _userSessionService.CreateUserClaims(username, displayName);
            await _userSessionService.SignInUserAsync(httpContext, claims);

            return username;
        }
    }
} 