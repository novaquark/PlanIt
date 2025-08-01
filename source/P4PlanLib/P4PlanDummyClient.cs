using GraphQL;
using P4PlanLib.Model;

namespace P4PlanLib
{
    public class P4PlanDummyClient : IP4PlanClient
    {
        public Task<string> ConnectedUserName()
        {
            return Task.FromResult<string>("test-username");
        }

        public Task<Item?> GetBacklogItem(string id)
        {
            return Task.FromResult<Item?>(new Item()
            {
                Name = "Test backlog item",
                Id = id
            });
        }

        public Task<Item?> GetBug(string id)
        {
            return Task.FromResult<Item?>(new Item()
            {
                Name = "Test bug",
                Id = id
            });
        }

        public Task<List<Comment>> GetComments(string itemId)
        {
            return Task.FromResult(new List<Comment>()
            {
                new Comment() { Id = "1", Text = "Test comment 1" },
                new Comment() { Id = "2", Text = "Test comment 2" }
            });
        }

        public Task<List<Item>> GetItemChildrenAsync(string backlogEntryId, bool includeCompletedTasks = false)
        {
            return Task.FromResult(new List<Item>()
            {
                new Item() { Id = "1", Name = "Test child task 1" },
                new Item() { Id = "2", Name = "Test child task 2" },
                new Item() { Id = "3", Name = "Test child task 3" },
                new Item() { Id = "4", Name = "Test child task 4" },
            });
        }

        public bool IsConnected()
        {
            return true;
        }

        public Task LoginAsync(string username, string password)
        {
            return Task.CompletedTask;
        }

        public Task<bool> PostComment(string itemId, string text)
        {
            return Task.FromResult(true);
        }

        public Task<T?> RunAsync<T>(GraphQLRequest request, string fieldPath)
        {
            throw new NotImplementedException();
        }

        public Task<List<Item>> Search(string query)
        {
            return Task.FromResult(new List<Item>());
        }
    }
}
