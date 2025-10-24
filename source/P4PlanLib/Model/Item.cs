using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace P4PlanLib.Model;

public class Item
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Temporary value to sort among multiple requests (like todolist)
    /// </summary>
    public int Rank { get; set; } = 0;

    /// <summary>
    /// Can be Backlog, Bug, Sprint, Showstopper
    /// </summary>
    public ItemType Type { get; set; }
    
    [JsonProperty("projectID")]
    public string ProjectID { get; set; } = string.Empty;

    [JsonProperty("localID")]
    public string LocalID { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("sprintPriority")]
    public string Priority { get; set; } = string.Empty;

    [JsonProperty("severity")]
    public string Severity { get; set; } = string.Empty;

    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;

    [JsonProperty("hyperlink")]
    public string Hyperlink { get; set; } = string.Empty;

    [JsonProperty("indentationLevel")]
    public int IndentationLevel { get; set; }

    [JsonProperty("itemLink")]
    public string ItemLink { get; set; } = string.Empty;

    [JsonProperty("subprojectPath")]
    public string SubprojectPath { get; set; } = string.Empty;

    [JsonProperty("status")]
    public string Status { get; set; } = string.Empty;

    [JsonProperty("estimatedDays")]
    public float EstimatedDays { get; set; }

    [JsonProperty("workRemaining")]
    public float? WorkRemaining { get; set; }

    [JsonProperty("committedTo")]
    public Sprint? CommittedTo { get; set; }

    [JsonProperty("assignedTo")]
    public AssignedTo[]? AssignedTo { get; set; }

    public string GetItemLink()
    {
        return $"https://novaquark.hansoft.cloud/task/3767a0ec/{Id}";
    }

    public string BeautifulPriority
    {
        get
        {
            return Priority switch
            {
                "veryLow" => "6 - Very Low",
                "low" => "5 - Low",
                "medium" => "3 - Medium",
                "high" => "2 - High",
                "veryHigh" => "1 - Very High",
                _ => "4 - No prio",
            };
        }
    }
}

public enum ItemType
{
    [Display(Name = "Scheduled Task")]
    ScheduledTask,
    [Display(Name = "Backlog Task")]
    BacklogTask,
    Bug,
    Sprint,
    Release
}