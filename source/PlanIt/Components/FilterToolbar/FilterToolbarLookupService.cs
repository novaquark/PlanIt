
using System.Security.Claims;
using PlanIt.Services;

namespace PlanIt.Components.FilterToolbar;
public class FilterToolbarLookupService : IFilterToolbarLookupService
{
    private readonly IP4PlanClientProvider _p4PlanClientProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public FilterToolbarLookupService(IP4PlanClientProvider p4PlanClientProvider, IHttpContextAccessor httpContextAccessor)
    {
        _p4PlanClientProvider = p4PlanClientProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    private string? GetCurrentUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
    }

    private string? GetCurrentUserName()
    {
        return _httpContextAccessor.HttpContext?.User?.Claims
            .FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }

    public async Task<IEnumerable<string>> GetPrioritiesAsync()
    {
         var client = _p4PlanClientProvider.GetP4PlanClient(GetCurrentUserEmail());
         return await client!.GetPrioritiesAsync();
    }

    public async Task<IEnumerable<string>> GetSprintsAsync()
    {
        var client = _p4PlanClientProvider.GetP4PlanClient(GetCurrentUserEmail());
        return await client!.GetSprintsAsync();
    }

    public async Task<IEnumerable<string>> GetStatusesAsync()
    {
        var client = _p4PlanClientProvider.GetP4PlanClient(GetCurrentUserEmail());
        return await client!.GetStatusesAsync();
    }

    public Task<string?> GetCurrentUserEmailAsync()
    {
        return Task.FromResult(GetCurrentUserEmail());
    }

    public async Task<string?> GetCurrentUserDisplayNameAsync()
    {
        var email = GetCurrentUserEmail();
        var client = _p4PlanClientProvider.GetP4PlanClient(email);
        try
        {
            var name = await client!.ConnectedUserName();
            if (!string.IsNullOrWhiteSpace(name))
                return name;
        }
        catch { }
        return _httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }
}
