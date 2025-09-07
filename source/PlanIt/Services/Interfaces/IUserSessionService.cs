using System.Security.Claims;

namespace PlanIt.Services.Interfaces
{
    /// <summary>
    /// Handles user session operations and claims management
    /// </summary>
    public interface IUserSessionService
    {
        /// <summary>
        /// Creates user claims for authentication
        /// </summary>
        /// <param name="username">User's email/username</param>
        /// <param name="displayName">User's display name</param>
        /// <returns>Claims principal for the user</returns>
        ClaimsPrincipal CreateUserClaims(string username, string displayName);

        /// <summary>
        /// Signs in a user with the specified claims
        /// </summary>
        /// <param name="httpContext">HTTP context for the request</param>
        /// <param name="claims">User claims</param>
        /// <returns>Task representing the async operation</returns>
        Task SignInUserAsync(HttpContext httpContext, ClaimsPrincipal claims);

        /// <summary>
        /// Signs out the current user
        /// </summary>
        /// <param name="httpContext">HTTP context for the request</param>
        /// <returns>Task representing the async operation</returns>
        Task SignOutUserAsync(HttpContext httpContext);
    }
} 