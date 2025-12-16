using GraphQL;
using P4PlanLib.Model;

namespace P4PlanLib;

public interface IP4PlanClient
{
    bool IsConnected();
    Task<string> ConnectedUserName();
    Task LoginAsync(string username, string password);
    Task<T?> RunAsync<T>(GraphQLRequest request, string fieldPath);
    Task<Item?> GetBacklogItem(string id);
    Task<Item?> GetBug(string id);
    Task<List<Item>> Search(string query);
    Task<List<Item>> GetTodoListAsync(string userID);
    Task<List<Comment>> GetComments(string itemId);
    Task<bool> PostComment(string itemId, string text);
    Task<List<Item>> GetItemChildrenAsync(string backlogEntryId, bool includeCompletedTasks = false);
    Task<IEnumerable<string>> GetPrioritiesAsync();
    Task<IEnumerable<string>> GetSprintsAsync();
    Task<IEnumerable<string>> GetAssigneesAsync(string? search);

}