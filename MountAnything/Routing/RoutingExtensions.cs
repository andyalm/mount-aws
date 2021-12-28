using System.Text.RegularExpressions;

namespace MountAnything.Routing;

public static class RoutingExtensions
{
    private const string ItemRegex = @"[a-z0-9-_\.]+";
    
    public static void Map<T>(this IRouter router, Action<Route>? createChildRoutes = null) where T : IPathHandler
    {
        router.MapRegex<T>(ItemRegex, createChildRoutes);
    }
    
    public static void Map<T>(this IRouter router, string routeValueName, Action<Route>? createChildRoutes = null) where T : IPathHandler
    {
        router.MapRegex<T>($"(?<{routeValueName}>{ItemRegex})", createChildRoutes);
    }

    public static void MapLiteral<T>(this IRouter router, string literal, Action<Route>? createChildRoutes = null)
        where T : IPathHandler
    {
        router.MapRegex<T>(Regex.Escape(literal), createChildRoutes);
    }
}