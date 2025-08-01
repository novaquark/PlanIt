using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class ItemDetails
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("workflowStatus")]
    public string WorkflowStatus { get; set; } = string.Empty;
}

