using MountAnything;

namespace MountAws;

internal static class LinqExtensions
{
    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
        }
    }

    public static T[] ToArrayOrEmpty<T>(this T? item) where T : class
    {
        return item == null ? Array.Empty<T>() : new[] { item };
    }

    public static IEnumerable<TValue> MultiGet<TKey, TValue>(this IDictionary<TKey, TValue> dictionary,
        IEnumerable<TKey> keys)
    {
        foreach (var key in keys)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                yield return value;
            }
        }
    }

    public static IEnumerable<T> WarnIfMoreItemsThan<T>(this IEnumerable<T> items, int cap, IPathHandlerContext context,
        string warningMessage)
    {
        var count = 1;
        foreach (var item in items)
        {
            yield return item;
            count += 1;
            if (count == cap)
            {
                context.WriteWarning(warningMessage);
            }
        }
    }
    
    public static string SnakeToPascalCase(this string word)
    {
        return string.Join("" , word.Split('_')
            .Select(w => w.Trim())
            .Where(w => w.Length > 0)
            .Select(w => w[..1].ToUpper() + w[1..].ToLower()));
    }
}