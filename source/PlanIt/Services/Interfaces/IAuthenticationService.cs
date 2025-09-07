using Microsoft.AspNetCore.Http;

namespace PlanIt.Services.Interfaces
{
    /// <summary>
    /// Handles authentication operations for PlanIt application
    /// </summary>
    public interface IAppAuthenticationService
    {
        /// <summary>
        /// Prepares a sign-in session for the specified user
        /// </summary>
        /// <param name="username">Username to prepare sign-in for</param>
        /// <returns>Unique identifier for the prepared sign-in session</returns>
        string PrepareSignIn(string username);

        /// <summary>
        /// Retrieves the username associated with a prepared sign-in session
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        /// <returns>Username if found, null otherwise</returns>
        string? GetPreparedSignIn(string sessionId);

        /// <summary>
        /// Cleans up expired sign-in sessions
        /// </summary>
        void CleanupExpiredSessions();

        /// <summary>
        /// Automatically authenticates a user (for development mode)
        /// </summary>
        /// <param name="httpContext">HTTP context for authentication</param>
        /// <returns>Username of the authenticated user</returns>
        Task<string> AutoAuthenticateAsync(HttpContext httpContext);
    }
} 