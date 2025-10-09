using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class User
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("fullName")]
    public string FullName { get; set; } = string.Empty;
}