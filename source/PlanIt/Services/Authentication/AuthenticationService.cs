using Microsoft.AspNetCore.Http;
using PlanIt.Services.Interfaces;

namespace PlanIt.Services.Authentication
{
    /// <summary>
    /// Handles authentication session management for PlanIt
    /// </summary>
    public class AuthenticationService : IAppAuthenticationService
    {
        private readonly Dictionary<string, SignInSession> _sessions = new();
        private readonly object _lockObject = new();
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(ILogger<AuthenticationService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string PrepareSignIn(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty", nameof(username));

            var sessionId = Guid.NewGuid().ToString();
            var session = new SignInSession
            {
                Username = username,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5) // 5 minute expiry
            };

            lock (_lockObject)
            {
                _sessions[sessionId] = session;
            }

            _logger.LogDebug("Prepared sign-in session {SessionId} for user {Username}", sessionId, username);
            return sessionId;
        }

        public string? GetPreparedSignIn(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return null;

            lock (_lockObject)
            {
                if (_sessions.TryGetValue(sessionId, out var session))
                {
                    if (session.ExpiresAt > DateTime.UtcNow)
                    {
                        _logger.LogDebug("Retrieved valid sign-in session {SessionId} for user {Username}", sessionId, session.Username);
                        return session.Username;
                    }
                    else
                    {
                        _sessions.Remove(sessionId);
                        _logger.LogDebug("Removed expired sign-in session {SessionId}", sessionId);
                    }
                }
            }

            return null;
        }

        public void CleanupExpiredSessions()
        {
            var expiredSessions = new List<string>();

            lock (_lockObject)
            {
                foreach (var kvp in _sessions)
                {
                    if (kvp.Value.ExpiresAt <= DateTime.UtcNow)
                    {
                        expiredSessions.Add(kvp.Key);
                    }
                }

                foreach (var sessionId in expiredSessions)
                {
                    _sessions.Remove(sessionId);
                }
            }

            if (expiredSessions.Count > 0)
            {
                _logger.LogDebug("Cleaned up {Count} expired sign-in sessions", expiredSessions.Count);
            }
        }

        public Task<string> AutoAuthenticateAsync(HttpContext httpContext)
        {
            // Auto-authentication is not supported in production mode
            throw new NotSupportedException("Auto-authentication is only available in development mode");
        }

        private class SignInSession
        {
            public string Username { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }
    }
} 