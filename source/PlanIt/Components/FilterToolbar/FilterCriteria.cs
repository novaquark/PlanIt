using System;
using System.Collections.Generic;

namespace PlanIt.Components.FilterToolbar;
public class FilterCriteria
{
    public bool OnlyMyTasks { get; set; }
    public bool Unfinished { get; set; } = true;
    public bool PriorityVeryHigh { get; set; }
    public bool PriorityHigh { get; set; }
    public bool PriorityOther { get; set; }
    public string? Sprint { get; set; }
    public string? Assignee { get; set; }

    public void Reset()
    {
        OnlyMyTasks = false;
        Unfinished = true;
        PriorityVeryHigh = false;
        PriorityHigh = false;
        PriorityOther = false;
        Sprint = null;
        Assignee = null;
    }
}
