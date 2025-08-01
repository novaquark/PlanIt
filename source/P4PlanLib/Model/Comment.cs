using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class Comment
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("item")]
    public Item? Item { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("postedBy")]
    public User? PostedBy { get; set; }

    [JsonProperty("postedAt")]
    public string PostedAt { get; set; } = string.Empty;

    [JsonProperty("mentionedUsers")]
    public User[]? MentionedUsers { get; set; }

    [JsonProperty("attachments")]
    public Attachment[]? Attachments { get; set; }
}