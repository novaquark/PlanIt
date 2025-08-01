using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class AssignedTo
{
    [JsonProperty("user")]
    public User? User { get; set; }
}
