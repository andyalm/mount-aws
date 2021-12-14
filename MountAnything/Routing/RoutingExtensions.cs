using System.Text.RegularExpressions;

namespace MountAnything.Routing;

public static class RoutingExtensions
{
    public static void Map<T>(this IRouter router, Action<Route>? createChildRoutes = null) where T : IPathHandler
    {
        router.MapRegex<T>("[a-z0-9-_]+", createChildRoutes);
    }

    public static void MapLiteral<T>(this IRouter router, string literal, Action<Route>? createChildRoutes = null)
        where T : IPathHandler
    {
        router.MapRegex<T>(Regex.Escape(literal), createChildRoutes);
    }
}