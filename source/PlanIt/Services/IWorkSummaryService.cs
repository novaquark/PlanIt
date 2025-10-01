namespace PlanIt.Services
{
    public interface IWorkSummaryService
    {
        int MajorBugs { get; set; }
        int MinorBugs { get; set; }
        int Tasks { get; set; }
        int DaysLeft { get; set; }

        event Action? OnChange;
        Task RefreshAsync(string userDisplayName, string userEmail);
    }

    public class WorkSummaryService(IP4PlanClientProvider clientProvider) : IWorkSummaryService
    {
        private readonly IP4PlanClientProvider _clientProvider = clientProvider;
        public int MajorBugs { get; set; } = 0;
        public int MinorBugs { get; set; } = 0;
        public int Tasks { get; set; } = 0;
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
                $"\"Item type\"=\"backlog item\" and Status!=Complete and \"Assigned to\":\"{userDisplayName}\" and \"Committed to\":\"S30\"");

            MajorBugs = myOpenShowstoppers.Count;
            MinorBugs = myOpenBugs.Count;
            Tasks = mySprintItems.Count;
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
