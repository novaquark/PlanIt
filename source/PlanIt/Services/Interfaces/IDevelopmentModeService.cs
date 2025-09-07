namespace PlanIt.Services.Interfaces
{
    /// <summary>
    /// Manages development mode configuration and settings
    /// </summary>
    public interface IDevelopmentModeService
    {
        /// <summary>
        /// Indicates whether the application is running in development mode
        /// </summary>
        bool IsDevelopmentMode { get; }

        /// <summary>
        /// Indicates whether dummy client should be used
        /// </summary>
        bool UseDummyClient { get; }

        /// <summary>
        /// Indicates whether authentication should be bypassed
        /// </summary>
        bool BypassAuthentication { get; }

        /// <summary>
        /// Gets the development user email
        /// </summary>
        string DevelopmentUserEmail { get; }

        /// <summary>
        /// Gets the development user display name
        /// </summary>
        string DevelopmentUserName { get; }
    }
} 