using PlanIt.Services.Interfaces;

namespace PlanIt.Services.Configuration
{
    /// <summary>
    /// Provides development mode configuration and user information
    /// </summary>
    public class DevelopmentModeService : IDevelopmentModeService
    {
        public bool IsDevelopmentMode => true; // Always true when this service is used

        public bool UseDummyClient => true; // Always true when this service is used

        public bool BypassAuthentication => true; // Always true when this service is used

        public string DevelopmentUserEmail => "dev-user@example.com";

        public string DevelopmentUserName => "Development User";
    }
} 