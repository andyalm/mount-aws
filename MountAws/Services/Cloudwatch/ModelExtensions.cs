using System.Collections;
using Amazon.CloudWatch.Model;

namespace MountAws.Services.Cloudwatch;

public static class ModelExtensions
{
    public static string[][] DimensionNames(
        this IEnumerable<Metric> metrics)
    {
        return metrics.Select(m => m.Dimensions.Select(d => d.Name).ToArray())
            .Distinct(new ArrayEqualityComparer()).ToArray();
    }
    
    public static string[] DimensionNames(
        this Metric metrics)
    {
        return metrics.Dimensions.Select(d => d.Name).ToArray();
    }

    public static IEnumerable<Metric> WhereDimensionNamesMatch(this IEnumerable<Metric> metrics, string[] dimensionNamesToMatch)
    {
        return metrics.Where(m =>
            m.DimensionNames().SequenceEqual(dimensionNamesToMatch, StringComparer.OrdinalIgnoreCase));
    }

    public static bool DimensionsMatch(this IEnumerable<Dimension> dimensions, IEnumerable<Dimension> otherDimensions)
    {
        return dimensions.SequenceEqual(otherDimensions, new DimensionEqualityComparer());
    }

    public static Metric? MatchingDimensionsOrDefault(this IEnumerable<Metric> metrics,
        IEnumerable<Dimension> dimensions)
    {
        return metrics.FirstOrDefault(m => m.Dimensions.DimensionsMatch(dimensions));
    }

    public static string DimensionItemName(this string[] dimensions) => string.Join(".", dimensions);
    
    private class ArrayEqualityComparer : IEqualityComparer<string[]>
    {
        public bool Equals(string[]? x, string[]? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null && y != null)
                return false;
            if (x != null && y == null)
                return false;
        
            return x!.SequenceEqual(y!, StringComparer.OrdinalIgnoreCase);
        }

        public int GetHashCode(string[] obj)
        {
            return ((IStructuralEquatable)obj).GetHashCode(StringComparer.OrdinalIgnoreCase);
        }
    }
    
    private class DimensionEqualityComparer : IEqualityComparer<Dimension>
    {
        public bool Equals(Dimension? x, Dimension? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name.Equals(y.Name, StringComparison.OrdinalIgnoreCase) && x.Value.Equals(y.Value, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(Dimension obj)
        {
            return HashCode.Combine(obj.Name?.ToLowerInvariant(), obj.Value?.ToLowerInvariant());
        }
    }
}

