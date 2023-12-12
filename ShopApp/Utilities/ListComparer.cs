using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace ShopApp.Utilities
{
    public class ListComparer : EqualityComparer<List<int>>
    {
        public override bool Equals(List<int> x, List<int> y)
        => StructuralComparisons.StructuralEqualityComparer.Equals(x?.ToArray(), y?.ToArray());

        public override int GetHashCode(List<int> x)
        => StructuralComparisons.StructuralEqualityComparer.GetHashCode(x.ToArray());
    }

    public class StringListComparer : EqualityComparer<List<string>>
    {
        public override bool Equals(List<string>? x, List<string>? y)
        => StructuralComparisons.StructuralEqualityComparer.Equals(x?.ToArray(), y?.ToArray());

        public override int GetHashCode(List<string> x)
        => StructuralComparisons.StructuralEqualityComparer.GetHashCode(x.ToArray());
    }
}
