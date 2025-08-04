using GraphQL;
using P4PlanLib.Model;
using System.Collections.Concurrent;

namespace P4PlanLib
{
    public class P4PlanDummyClient : IP4PlanClient
    {
        private readonly string _connectedUser = "test-username";

        private readonly ConcurrentDictionary<string, Item> _items = new();
        private readonly ConcurrentDictionary<string, List<Comment>> _comments = new();

        private readonly string[] _priorities = new[] { "Low", "Medium", "High", "Critical" };
        private readonly string[] _sprints = new[] { "Backlog", "Sprint 1", "Sprint 2" };

        public P4PlanDummyClient()
        {
            GenerateFakeData();
        }

        private void GenerateFakeData()
        {
            var random = new Random();

            for (int i = 1; i <= 15; i++)
            {
                var id = $"BI-{i}";
                var item = new Item
                {
                    Id = id,
                    Name = $"Backlog Item {i}",
                    Description = $"This is a dummy description for item {i}.",
                    Status = i % 3 == 0 ? "Done" : (i % 2 == 0 ? "In Progress" : "New"),
                    AssignedTo = [new AssignedTo { User = new User() { Name = i % 2 == 0 ? "alice@example.com" : "bob@example.com" } }],
                    Priority = _priorities[random.Next(_priorities.Length)],
                    CommittedTo = new Sprint { Name = _sprints[random.Next(_sprints.Length)] },
                };

                _items[id] = item;

                _comments[id] = new List<Comment>
                {
                    new Comment { Id = $"{id}-C1", Text = $"Initial comment on {id}" },
                    new Comment { Id = $"{id}-C2", Text = $"Second comment on {id}" }
                };
            }
        }

        public Task<string> ConnectedUserName()
        {
            return Task.FromResult(_connectedUser);
        }

        public Task<Item?> GetBacklogItem(string id)
        {
            _items.TryGetValue(id, out var item);
            return Task.FromResult(item);
        }

        public Task<Item?> GetBug(string id)
        {
            return GetBacklogItem(id);
        }

        public Task<List<Comment>> GetComments(string itemId)
        {
            if (_comments.TryGetValue(itemId, out var comments))
                return Task.FromResult(comments.ToList());

            return Task.FromResult(new List<Comment>());
        }

        public Task<List<Item>> GetItemChildrenAsync(string backlogEntryId, bool includeCompletedTasks = false)
        {
            var children = Enumerable.Range(1, 3).Select(i => new Item
            {
                Id = $"{backlogEntryId}-T{i}",
                Name = $"Task {i} for {backlogEntryId}",
                Status = i % 2 == 0 ? "Done" : "To Do",
                AssignedTo = [new AssignedTo { User = new User() { Name = "charlie@example.com" } }],
                CommittedTo = new Sprint { Name = "Sprint 2" },
                Priority = _priorities[i % _priorities.Length],
            }).ToList();

            return Task.FromResult(children);
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
            if (!_comments.ContainsKey(itemId))
                _comments[itemId] = new List<Comment>();

            var newComment = new Comment
            {
                Id = $"{itemId}-C{_comments[itemId].Count + 1}",
                Text = text
            };

            _comments[itemId].Add(newComment);
            return Task.FromResult(true);
        }

        public Task<T?> RunAsync<T>(GraphQLRequest request, string fieldPath)
        {
            throw new NotImplementedException("Dummy client does not support raw GraphQL.");
        }

        public Task<List<Item>> Search(string query)
        {
            var result = _items.Values
                .ToList();

            return Task.FromResult(result);
        }
    }
}
