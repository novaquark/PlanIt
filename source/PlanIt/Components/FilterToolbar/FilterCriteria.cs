using System;
using System.Collections.Generic;

namespace PlanIt.Components.FilterToolbar;
public class FilterCriteria
{
    public bool OnlyMyTasks { get; set; }
    public bool TopPriority { get; set; }
    public string? Priority { get; set; }
    public string? Sprint { get; set; }
    public string? Assignee { get; set; }
    public string? Committed { get; set; }
    public string? Status { get; set; }
    
    public void Reset()
    {
        OnlyMyTasks = false;
        TopPriority = false;
        Priority = null;
        Sprint = null;
        Status = null;
        Assignee = null;
        Committed = null;
    }
}
