using MountAnything;

namespace MountAws;

internal static class LinqExtensions
{
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (var item in enumerable)
        {
            action(item);
            yield return item;
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
}