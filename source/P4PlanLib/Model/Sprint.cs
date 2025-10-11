using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class Sprint : Item
{

    [JsonProperty("localId")]
    public string LocalId { get; set; } = string.Empty;

    public bool IsCurrentSprint()
    {
        var now = DateTime.Now;
        return StartDate <= now && now <= EndDate;
    }

    public bool IsPastSprint()
    {
        return EndDate < DateTime.Now;
    }

    public bool IsFutureSprint()
    {
        return StartDate > DateTime.Now;
    }
}
