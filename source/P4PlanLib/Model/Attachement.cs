using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class Attachment
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("path")]
    public string Path { get; set; } = string.Empty;

    [JsonProperty("size")]
    public long Size { get; set; }

    [JsonProperty("version")]
    public int Version { get; set; }

    [JsonProperty("imageWidth")]
    public int ImageWidth { get; set; }

    [JsonProperty("imageHeight")]
    public int ImageHeight { get; set; }

    [JsonProperty("coverImage")]
    public string CoverImage { get; set; } = string.Empty;

    [JsonProperty("addedBy")]
    public User? AddedBy { get; set; }
}