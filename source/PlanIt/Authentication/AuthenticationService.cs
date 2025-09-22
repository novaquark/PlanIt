
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace PlanIt.Authentication
{
    
    public interface IAuthenticationService
    {
        string PrepareSignIn(string username);
        string? GetPreparedSignIn(string id);
        ClaimsPrincipal CreateUserClaims(string username, string displayName);
    }
    
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Dictionary<string, string> _preparedSignIns = [];
        private readonly object _signInLock = new();

        public string? GetPreparedSignIn(string id)
        {
            string? value = null;
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
    }
}
