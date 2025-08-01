using P4PlanLib;

namespace PlanIt.Services;

public interface IP4PlanClientProvider
{
    // Gets a client reference valid thoughout user session based on user account display name once connected
    // If not connected or connexion has timeout, result will be null
    IP4PlanClient? GetP4PlanClient(string? email);
    Task<IP4PlanClient?> Connect(string email, string password);
}

public class P4PlanClientProvider(string graphQlUrl, string projectIdWhitelist) : IP4PlanClientProvider
{
    private readonly List<(string email, IP4PlanClient client)> _clients = [];

    public async Task<IP4PlanClient?> Connect(string email, string password)
    {
        var p4PlanClient = new P4PlanClient(graphQlUrl, projectIdWhitelist);
        await p4PlanClient.LoginAsync(email, password);
        if (p4PlanClient.IsConnected())
        {
            // clean previous clients of the same user
            _clients.RemoveAll(c => c.email == email);
            _clients.Add((email, p4PlanClient));
            return p4PlanClient;
        }

        return null;
    }

    public IP4PlanClient? GetP4PlanClient(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return null;

        return _clients.FirstOrDefault(x => x.email == email).client;
    }
}

public class P4PlanDummyClientProvider : IP4PlanClientProvider
{
    public Task<IP4PlanClient?> Connect(string email, string password)
    {
        return Task.FromResult<IP4PlanClient?>(new P4PlanDummyClient());
    }

    public IP4PlanClient? GetP4PlanClient(string? email)
    {
        return new P4PlanDummyClient();
    }
}
