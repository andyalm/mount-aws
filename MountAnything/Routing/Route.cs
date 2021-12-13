using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Autofac;

namespace MountAnything.Routing;

public class Route
{
    private IEnumerable<Action<Match, ContainerBuilder>> _serviceRegistrations;
    private readonly List<Route> _childRoutes = new();
    public string Pattern { get; }
    public Regex Regex { get; }
    public Type HandlerType { get; }

    public Route(string regex, Type handlerType, IEnumerable<Action<Match,ContainerBuilder>>? serviceRegistrations = null)
    {
        Pattern = regex;
        Regex = new Regex("^" + regex + "$", RegexOptions.IgnoreCase);
        HandlerType = handlerType;
        _serviceRegistrations = serviceRegistrations ?? Enumerable.Empty<Action<Match, ContainerBuilder>>();
    }
    
    public bool TryMatch(string path, out RouteMatch match)
    {
        var regexMatch = Regex.Match(path);
        if (regexMatch.Success)
        {
            match = ToRouteMatch(path, regexMatch);
            return true;
        }

        foreach (var childRoute in _childRoutes)
        {
            if (childRoute.TryMatch(path, out var childMatch))
            {
                match = childMatch;
                return true;
            }
        }

        match = default!;
        return false;
    }

    public void RegisterServices(Action<Match, ContainerBuilder> serviceRegistration)
    {
        _serviceRegistrations = _serviceRegistrations.Concat(new[] { serviceRegistration });
    }

    public void MapRegex<THandler>(string pattern, Action<Route>? createChildRoutes = null) where THandler : IPathHandler
    {
        var fullPattern = $"{Pattern}/{pattern}";
        var route = new Route(fullPattern, typeof(THandler), _serviceRegistrations);
        createChildRoutes?.Invoke(route);
        _childRoutes.Add(route);
    }

    private RouteMatch ToRouteMatch(string path, Match regexMatch)
    {
        return new RouteMatch(path, HandlerType)
        {
            Values = regexMatch.Groups.Keys
                .Select(key => new KeyValuePair<string, string>(key, regexMatch.Groups[key].Value))
                .ToImmutableDictionary(StringComparer.OrdinalIgnoreCase),
            ServiceRegistrations = (builder) =>
            {
                foreach (var serviceRegistration in _serviceRegistrations)
                {
                    serviceRegistration.Invoke(regexMatch, builder);
                }
            }
        };
    }
}