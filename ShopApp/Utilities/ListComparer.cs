using System.Collections;
using System.Collections.Generic;

namespace ShopApp.Utilities
{
    public class ListComparer : EqualityComparer<List<int>>
    {
        public override bool Equals(List<int> x, List<int> y)
        => StructuralComparisons.StructuralEqualityComparer.Equals(x?.ToArray(), y?.ToArray());

        public override int GetHashCode(List<int> x)
        => StructuralComparisons.StructuralEqualityComparer.GetHashCode(x.ToArray());
    }
}
