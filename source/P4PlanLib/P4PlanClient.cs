using System.Net.Http.Headers;
using GraphQL;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using Newtonsoft.Json.Linq;
using P4PlanLib.Model;

namespace P4PlanLib;

public class P4PlanClient : IP4PlanClient
{
    #region Core client features

    /// <summary>
    /// Main constructor for P4PlanClient, needing project url and whitelist of project to query (not all projects by default)
    /// </summary>
    /// <param name="graphQlUrl">GraphQL endpoint of the P4Plan server, example: https://company-name.hansoft.cloud/api/graphql</param>
    /// <param name="projectIdWhitelist">Comma separated project ids, example: 123,456,789</param>
    public P4PlanClient(string graphQlUrl, string projectIdWhitelist)
    {
        ServerUrl = graphQlUrl;
        _client = new GraphQLHttpClient(ServerUrl, new NewtonsoftJsonSerializer());
        _projectWhitelist = new List<string>(projectIdWhitelist.Split(','));
    }

    private readonly List<string> _projectWhitelist;
    private readonly string ServerUrl;
    private string _token = string.Empty;
    private readonly GraphQLHttpClient _client;
    private bool _connected = false;
    private string _connectedUsername = string.Empty;
    private IEnumerable<string>? _sprintsCache;

    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);
    private DateTime? _sprintsCachedAtUtc;
    private DateTime? _assigneesCachedAtUtc;


    private IEnumerable<string>? _assigneesCache;

    public async Task LoginAsync(string username, string password)
    {
        _token = string.Empty;
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return;
        }

        var request = new GraphQLRequest
        {
            Query = @"mutation login($loginUserInput: LoginUserInput!) {
  login(loginUserInput: $loginUserInput) {
    access_token
  }
}",
            Variables = new
            {
                loginUserInput = new
                {
                    username,
                    password,
                }
            }
        };

        var response = await _client.SendQueryAsync<dynamic>(request);
        _token = response.Data.login.access_token;
        _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        _connected = true;
    }

    public async Task<T?> RunAsync<T>(GraphQLRequest request, string fieldPath)
    {
        if (!IsConnected())
        {
            return default;
        }

        var response = await _client.SendQueryAsync<dynamic>(request);

        if (response.Data is null)
            return default;

        var token = SelectToken(response.Data, fieldPath);
        if (token is null)
            return default;

        return token.ToObject<T>();
    }

    private static JToken? SelectToken(dynamic data, string fieldPath)
    {
        try
        {
            var jObject = data as JObject;
            return jObject?.SelectToken(fieldPath);
        }
        catch
        {
            return null;
        }
    }

    public bool IsConnected()
    {
        return _connected;
    }

    // FIXME TODO This implementation needs improvements
    // I didn't found direct GraphQL API call to retrieve this information
    // This current implementation is slow and fails if the user don't have an assigned item on which he is the sole assignee
    public async Task<string> ConnectedUserName()
    {
        if (!_connected)
            return string.Empty;

        if (!string.IsNullOrEmpty(_connectedUsername))
            return _connectedUsername;

        var request = new GraphQLRequest
        {
            Query = @"query {
                        todoList {
                            id
                            name
                        }
                    }"
        };

        var response = await RunAsync<IEnumerable<Item>>(request, "todoList");

        if (response is null)
            return string.Empty;

        foreach (var item in response)
        {
            var backlogItem = await GetBacklogItem(item.Id);
            if (backlogItem is null)
                continue;

            if (backlogItem.AssignedTo is null || backlogItem.AssignedTo.Length != 1)
                continue;

            _connectedUsername = backlogItem.AssignedTo[0].User.Name;
            return _connectedUsername;
        }

        return string.Empty;
    }

    #endregion

    #region Requests

    public async Task<Item?> GetBacklogItem(string id)
    {
        var MutationQuery_UpdateBacklogTask = @"mutation updateBacklogTask($updateBacklogTaskInput: UpdateBacklogTaskInput!) {
  updateBacklogTask(updateBacklogTaskInput: $updateBacklogTaskInput) {
    id
    projectID
    localID
    committedToProjectID
    name
    createdBy {
      id
      name
    }
    createdOn
    status
    workflow {
      id
      projectID
      name
    }
    confidence
    risk
    color
    hyperlink
    assignedTo {
      user {
        id
        name
      }
    }
    subprojectPath
    points
    indentationLevel
    workRemaining
    committedTo {
      id
      projectID
      localID
      name
      createdOn
      lastUpdatedOn
      lastCommentedOn
      confidence
      color
      risk
      hyperlink
      indentationLevel
      itemLink
      start
      finish
      hidden
      board
      duration
      watching
      membersHaveFullAccessRights
      workRemaining
      estimatedDays
      status
      points
      subprojectPath
    }
    itemLink
    category
    sprintPriority
    backlogPriority
    estimatedDays
    hidden
    boardLane {
      id
      name
    }
    boardColumn {
      id
      name
    }
    boardMembershipInfo {
      position
      hidden
    }
    epic
    isUserStory
    userStory
    wbs
    watching
    createdFromWorkflow
    hasChildren
  }
}";
        var Query_ItemDetails = @"query itemDetail($url: String!) {
  itemDetail(url: $url) {
    id
    name
    description
    status
    workflowStatus
  }
}";

        var request = new GraphQLRequest
        {
            Query = MutationQuery_UpdateBacklogTask,
            Variables = new
            {
                updateBacklogTaskInput = new
                {
                    id = id
                }
            }
        };

        var response = await RunAsync<Item>(request, "updateBacklogTask");

        if (response is null)
        {
            return null;
        }

        var detailsRequest = new GraphQLRequest
        {
            Query = Query_ItemDetails,
            Variables = new
            {
                url = response.ItemLink
            }
        };

        var detailedResponse = await RunAsync<ItemDetails>(detailsRequest, "itemDetail");

        if (detailedResponse != null)
        {
            response.Description = detailedResponse.Description;
            response.Status = detailedResponse.Status;
        }

        return response;
    }

    public async Task<Item?> GetBug(string id)
    {
        var MutationQuery_UpdateBug = @"mutation updateBug($updateBugInput: UpdateBugInput!) {
  updateBug(updateBugInput: $updateBugInput) {
    id
    projectID
    localID
    committedToProjectID
    name
    createdOn
    lastUpdatedOn
    lastCommentedOn
    status
    workflow {
      id
      projectID
      name
      icon
    }
    workflowStatus {
      id
      workflowID
      projectID
      name
      icon
      statusToWorkflowStatuses {
        status
        workflowStatusID
      }
    }
    assignedTo {
      user {
        id
        name
      }
      percentageAllocation
    }
    confidence
    risk
    severity
    color
    hyperlink
    subprojectPath
    indentationLevel
    workRemaining
    committedTo {
      id
      projectID
      localID
      name
      createdOn
      lastUpdatedOn
      lastCommentedOn
      confidence
      color
      risk
      hyperlink
      indentationLevel
      itemLink
      start
      finish
      hidden
      board
      duration
      watching
      membersHaveFullAccessRights
      workRemaining
      estimatedDays
      status
      points
      subprojectPath
    }
    itemLink
    stepsToReproduce
    detailedDescription
    bugPriority
    sprintPriority
    hidden
    boardLane {
      id
      name
    }
    boardColumn {
      id
      name
    }
    boardMembershipInfo {
      position
      hidden
    }
    watching
  }
}";
        var Query_ItemDetails = @"query itemDetail($url: String!) {
  itemDetail(url: $url) {
    id
    name
    description
    status
    workflowStatus
  }
}";

        var request = new GraphQLRequest
        {
            Query = MutationQuery_UpdateBug,
            Variables = new
            {
                updateBugInput = new
                {
                    id = id
                }
            }
        };

        var response = await RunAsync<Item>(request, "updateBug");

        if (response is null)
        {
            return null;
        }

        var detailsRequest = new GraphQLRequest
        {
            Query = Query_ItemDetails,
            Variables = new
            {
                url = response.ItemLink
            }
        };

        var detailedResponse = await RunAsync<ItemDetails>(detailsRequest, "itemDetail");

        if (detailedResponse != null)
        {
            response.Description = detailedResponse.Description;
            response.Status = detailedResponse.Status;
        }

        return response;
    }

    public async Task<List<Item>> Search(string query)
    {
        var queryItems = @"query items(
  $id: ID!,
  $findQuery: String,
  $limit: Int
) {
  items(
    id: $id,
    findQuery: $findQuery,
    limit: $limit
  ) {
    id
    projectID
    localID
  }
}";

        var request = new GraphQLRequest
        {
            Query = queryItems,
            Variables = new
            {
                id = "",
                findQuery = query
            }
        };

        // Query only asks for id
        var response = await RunAsync<IEnumerable<Item>>(request, "items");

        if (response is null)
        {
            return [];
        }

        var queryResults = new List<Item>();

        foreach (var responseItem in response)
        {
            if (!_projectWhitelist.Contains(responseItem.ProjectID))
                continue;

            var item = await GetBacklogItem(responseItem.Id);
            if (item != null)
            {
                queryResults.Add(item);
            }
            else
            {
                // Try Bug
                var bug = await GetBug(responseItem.Id);
                if (bug != null)
                {
                    queryResults.Add(bug);
                }
            }
        }

        return queryResults;
    }

    public async Task<List<Comment>> GetComments(string itemId)
    {
        var query = @"query comments($id: ID!) {
  comments(id: $id) {
    id
    item {
      id
      name
    }
    text
    postedBy {
      id
      name
    }
    postedAt
    mentionedUsers {
      id
      name
    }
  }
}";
        var request = new GraphQLRequest
        {
            Query = query,
            Variables = new { id = itemId }
        };
        var response = await RunAsync<List<Comment>>(request, "comments");
        return response ?? new List<Comment>();
    }

    public async Task<bool> PostComment(string itemId, string text)
    {
        var mutation = @"mutation postComment($input: PostCommentInput!) {
  postComment(postCommentInput: $input) {
    id
  }
}";
        var request = new GraphQLRequest
        {
            Query = mutation,
            Variables = new
            {
                input = new
                {
                    itemID = itemId,
                    text = text
                }
            }
        };
        var response = await _client.SendMutationAsync<dynamic>(request);
        return response.Data != null && response.Data?.postComment != null && response.Data?.postComment.id != null;
    }

    public async Task<List<Item>> GetItemChildrenAsync(string backlogEntryId, bool includeCompletedTasks = false)
    {
        var parentItem = await GetBacklogItem(backlogEntryId);
        if (parentItem == null)
            return new List<Item>();

        var subprojectPath = parentItem.SubprojectPath;
        var name = parentItem.Name;
        var searchQuery = $"subprojectPath = \"{subprojectPath}: {name}\"";
        if (!includeCompletedTasks)
        {
            searchQuery += " AND Status != Completed";
        }

        var children = await Search(searchQuery);
        return children;
    }

    public Task<IEnumerable<string>> GetPrioritiesAsync()
    {
        // TODO: Implement actual GraphQL query to get priorities from P4Plan server
        var priorities = new List<string>
        {
            "veryHigh",
            "high",
            "medium",
            "low",
            "veryLow"
        };
        return Task.FromResult<IEnumerable<string>>(priorities);
    }

    // CODE QUALITY DEBT
    public async Task<IEnumerable<string>> GetSprintsAsync()
    {
        if (_sprintsCache != null && _sprintsCachedAtUtc.HasValue && DateTime.UtcNow - _sprintsCachedAtUtc.Value < CacheTtl)
            return _sprintsCache;

        var items = await Search("");
        var sprints = items
            .Select(i => i.CommittedTo?.Name)
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(n => n!)
            .Distinct()
            .OrderBy(n => n)
            .ToList();

        _sprintsCache = sprints;
        _sprintsCachedAtUtc = DateTime.UtcNow;
        return sprints;
    }

    // CODE QUALITY DEBT
    public async Task<IEnumerable<string>> GetAssigneesAsync(string? search)
    {
        if (_assigneesCache == null || !_assigneesCachedAtUtc.HasValue || DateTime.UtcNow - _assigneesCachedAtUtc.Value >= CacheTtl)
        {
            var items = await Search("");
            _assigneesCache = items
                .SelectMany(i => i.AssignedTo ?? Array.Empty<AssignedTo>())
                .Select(a => a.User?.Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))
                .Select(n => n!)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(n => n)
                .ToList();
            _assigneesCachedAtUtc = DateTime.UtcNow;
        }

        var names = _assigneesCache.AsEnumerable();



        if (!string.IsNullOrWhiteSpace(search))
        {
            names = names.Where(n => n.Contains(search, StringComparison.OrdinalIgnoreCase));
        }
        return names.ToList();
    }
  #endregion

   // Invalidation helpers you can call to force refresh
   public void InvalidateSprintsCache() { _sprintsCache = null; _sprintsCachedAtUtc = null; }
    public void InvalidateAssigneesCache() { _assigneesCache = null; _assigneesCachedAtUtc = null; }

}

