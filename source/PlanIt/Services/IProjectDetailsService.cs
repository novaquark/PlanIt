namespace PlanIt.Services
{
    public interface IProjectDetailsService
    {
        string NextMilestoneName { get; set; }
        string CurrentSprint { get; set; }
    }

    public class ProjectDetailsService : IProjectDetailsService
    {
        public string NextMilestoneName { get; set; } = "None";
        public string CurrentSprint { get; set; } = "S30";
    }
}
