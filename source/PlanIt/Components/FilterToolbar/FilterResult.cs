using System.Collections.Generic;

namespace PlanIt.Components.FilterToolbar;
public class FilterResult<TModel>
{
    public FilterCriteria Criteria { get; set; } = new();
    public List<TModel> FilteredItems { get; set; } = new();
}
