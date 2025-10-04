
namespace PlanIt.Components.FilterToolbar;
public interface IFilterToolbarLookupService
{
    Task<IEnumerable<string>> GetPrioritiesAsync();
    Task<IEnumerable<string>> GetSprintsAsync();
    Task<IEnumerable<string>> GetStatusesAsync();
    Task<string?> GetCurrentUserEmailAsync();
    Task<string?> GetCurrentUserDisplayNameAsync();
}

