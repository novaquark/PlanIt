using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PlanIt.Services.Interfaces;
using IAuthenticationService = PlanIt.Services.Interfaces.IAuthenticationService;

namespace PlanIt.Authentication
{
    public class AuthenticationService : IAuthenticationService
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
    }
}
