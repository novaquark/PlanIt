using System.Diagnostics.CodeAnalysis;
using P4PlanLib.Model;

namespace P4PlanLib;

public class ItemComparer : IEqualityComparer<Item>
{
    public bool Equals(Item? x, Item? y)
    {
        if (x == null || y == null) 
            return false;
        
        return string.Compare(x.Id, y.Id, StringComparison.OrdinalIgnoreCase) == 0;
    }

    public int GetHashCode([DisallowNull] Item obj)
    {
        return obj.Id.GetHashCode();
    }
}
