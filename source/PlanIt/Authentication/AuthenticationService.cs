using Microsoft.AspNetCore.Http;
using PlanIt.Services.Interfaces;

namespace PlanIt.Authentication
{
    public interface IAuthenticationService
    {
        string PrepareSignIn(string username);
        string? GetPreparedSignIn(string id);
    }

    public class AuthenticationService : IAuthenticationService, IAppAuthenticationService
    {
        private readonly Dictionary<string, string> _preparedSignIns = [];
        private readonly object _signInLock = new();

        public string? GetPreparedSignIn(string id)
        {
            string value = string.Empty;
            lock (_signInLock)
            {
                _preparedSignIns.TryGetValue(id, out value);
            }
            return value;
        }

        public string PrepareSignIn(string username)
        {
            Guid guid;
            
            lock (_signInLock)
            {
                guid = Guid.NewGuid();
                _preparedSignIns[guid.ToString()] = username;
            }

            return guid.ToString();
        }

        public void CleanupExpiredSessions()
        {
            // In production mode, we might want to implement session cleanup
            // For now, we'll keep it simple
        }

        public Task<string> AutoAuthenticateAsync(HttpContext httpContext)
        {
            // Auto-authentication is not supported in production mode
            throw new NotSupportedException("Auto-authentication is only available in development mode");
        }
    }
}
