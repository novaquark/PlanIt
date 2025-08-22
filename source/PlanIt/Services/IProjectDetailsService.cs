namespace PlanIt.Services
{
    public interface IProjectDetailsService
    {
        string NextMilestoneName { get; set; }
    }

    public class ProjectDetailsService : IProjectDetailsService
    {
        public string NextMilestoneName { get; set; } = "None";
    }
}
