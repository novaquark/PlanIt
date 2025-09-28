namespace PlanIt.Services
{
    public interface ITodoListSummaryService
    {
        int MajorBugs { get; set; }
        int MinorBugs { get; set; }
        int Tasks { get; set; }
        int DaysLeft { get; set; }

        event Action? OnChange;
        void NotifyStateChanged();
    }

    public class TodoListSummaryService : ITodoListSummaryService
    {
        public int MajorBugs { get; set; } = 0;
        public int MinorBugs { get; set; } = 0;
        public int Tasks { get; set; } = 0;
        public int DaysLeft { get; set; } = 0;

        // notify when data changes
        public event Action? OnChange;

        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
