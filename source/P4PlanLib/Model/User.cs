using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class User
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}