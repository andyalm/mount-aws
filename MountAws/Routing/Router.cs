using System.Text.RegularExpressions;

namespace MountAws.Routing;

public class Router
{
    public static Router Instance { get; } = new();

    private readonly List<Route> _routes = new();

    public Router MapRegex<T>(string regex, Action<Route>? createChildRoutes = null) where T : IPathHandler
    {
        var route = new Route(regex, typeof(T));
        createChildRoutes?.Invoke(route);
        _routes.Add(route);

        return this;
    }

    public RouteMatch Match(string path)
    {
        foreach (var route in _routes)
        {
            if (route.TryMatch(path, out var match))
            {
                return match;
            }
        }

        throw new RoutingException($"No route matches path '{path}'");
    }
}