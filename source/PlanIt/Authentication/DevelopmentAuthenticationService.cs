using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace PlanIt.Authentication
{
    public class DevelopmentAuthenticationService : IAuthenticationService
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

        /// <summary>
        /// Automatically authenticate a user in development mode
        /// </summary>
        public async Task<string> AutoAuthenticateAsync(HttpContext httpContext)
        {
            var username = Environment.GetEnvironmentVariable("DUMMY_USER_EMAIL") ?? "dev-user@example.com";
            var displayName = Environment.GetEnvironmentVariable("DUMMY_USER_NAME") ?? "Development User";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Email, username),
                new Claim(ClaimTypes.NameIdentifier, username.Split('@')[0]),
                new Claim(ClaimTypes.Role, "ConnectedUser"),
                new Claim("DevelopmentMode", "true")
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(principal);
            
            return username;
        }
    }
} 