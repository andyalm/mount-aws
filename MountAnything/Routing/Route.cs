using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Autofac;

namespace MountAnything.Routing;

public class Route : IRoutable
{
    private Action<RouteMatch, ContainerBuilder> _serviceRegistrations;
    private readonly List<Route> _childRoutes = new();
    public string Pattern { get; }
    public Regex Regex { get; }
    public Type HandlerType { get; }

    public Route(string regex, Type handlerType, Action<RouteMatch,ContainerBuilder>? serviceRegistrations = null)
    {
        Pattern = regex;
        Regex = new Regex("^" + regex + "$", RegexOptions.IgnoreCase);
        HandlerType = handlerType;
        _serviceRegistrations = serviceRegistrations ?? ((_, _) => {});
    }
    
    public bool TryGetResolver(ItemPath path, out HandlerResolver resolver)
    {
        foreach (var childRoute in _childRoutes)
        {
            if (childRoute.TryGetResolver(path, out var childServiceRegistrations))
            {
                resolver = childServiceRegistrations;
                return true;
            }
        }
        
        var regexMatch = Regex.Match(path.FullName);
        if (regexMatch.Success)
        {
            var routeMatch = new RouteMatch(path, HandlerType)
            {
                Values = regexMatch.Groups.Keys
                    .Select(key => new KeyValuePair<string, string>(key, regexMatch.Groups[key].Value))
                    .ToImmutableDictionary(StringComparer.OrdinalIgnoreCase)
            };
            resolver = GetResolver(routeMatch);
            return true;
        }

        resolver = default!;
        return false;
    }

    public void RegisterServices(Action<RouteMatch, ContainerBuilder> serviceRegistration)
    {
        _serviceRegistrations += serviceRegistration;
    }

    public void MapRegex<THandler>(string pattern, Action<Route>? createChildRoutes = null) where THandler : IPathHandler
    {
        var fullPattern = $"{Pattern}/{pattern}";
        var route = new Route(fullPattern, typeof(THandler), _serviceRegistrations);
        createChildRoutes?.Invoke(route);
        _childRoutes.Add(route);
    }

    private HandlerResolver GetResolver(RouteMatch match)
    {
        return new HandlerResolver(
            match.HandlerType,
            (builder) =>
            {
                _serviceRegistrations.Invoke(match, builder);
            });
    }
}