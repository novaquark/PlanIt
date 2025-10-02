namespace PlanIt.Services
{
    public interface IWorkSummaryService
    {
        int MajorBugs { get; set; }
        int MinorBugs { get; set; }
        int TasksInSprint { get; set; }
        int TasksInBacklog { get; set; }
        int DaysLeft { get; set; }

        event Action? OnChange;
        Task RefreshAsync(string userDisplayName, string userEmail);
    }

    public class WorkSummaryService(IP4PlanClientProvider clientProvider,IProjectDetailsService projectDetailsService) : IWorkSummaryService
    {
        private readonly IP4PlanClientProvider _clientProvider = clientProvider;
        private readonly IProjectDetailsService _projectDetailsService = projectDetailsService;
        public int MajorBugs { get; set; } = 0;
        public int MinorBugs { get; set; } = 0;
        public int TasksInSprint { get; set; } = 0;
        public int TasksInBacklog { get; set; } = 0;
        public int DaysLeft { get; set; } = 0;

        // notify when data changes
        public event Action? OnChange;

        public async Task RefreshAsync(string userDisplayName, string userEmail)
        {
            var p4PlanClient = _clientProvider.GetP4PlanClient(userEmail);
            if (p4PlanClient is null) return;

            var comparer = new P4PlanLib.ItemComparer();

            var myOpenBugs = await p4PlanClient.Search(
                $"\"Item type\"=bug and Status!=Complete and Severity >\"Severity B\" and \"Assigned to\":\"{userDisplayName}\"");

            var myOpenShowstoppers = await p4PlanClient.Search(
                $"\"Item type\"=bug and Status!=Complete and Severity <=\"Severity B\" and \"Assigned to\":\"{userDisplayName}\"");

            var mySprintItems = await p4PlanClient.Search(
                $"\"Item type\"=\"backlog item\" and Status!=Complete and \"Assigned to\":\"{userDisplayName}\" and \"Committed to\":\"{_projectDetailsService.CurrentSprint}\"");

            var myBacklog = await p4PlanClient.Search($"\"Item type\"=\"backlog item\" and Status!=Complete and \"Assign tag\":\"{userDisplayName}\"");
            
            MajorBugs = myOpenShowstoppers.Count;
            MinorBugs = myOpenBugs.Count;
            TasksInSprint = mySprintItems.Count;
            TasksInBacklog = myBacklog.Count;
            DaysLeft = CalculateDaysLeft();

            OnChange?.Invoke();
        }

        private int CalculateDaysLeft()
        {
            var sprintEnd = new DateTime(2025, 10, 15);
            var today = DateTime.Today;
            var daysLeft = (sprintEnd - today).Days;
            return daysLeft < 0 ? 0 : daysLeft;
        }
    }
}
