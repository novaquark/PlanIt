using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class Sprint
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("localId")]
    public string LocalId { get; set; } = string.Empty;
    
    [JsonProperty("itemLink")]
    public string ItemLink { get; set; } = string.Empty;
}
