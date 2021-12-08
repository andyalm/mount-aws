using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Autofac;

namespace MountAws.Routing;

public record RouteMatch(string Path, Type HandlerType)
{
    public ImmutableDictionary<string, string> Values { get; init; } = ImmutableDictionary<string, string>.Empty;
    public IEnumerable<Action<Match,ContainerBuilder>> ServiceRegistrations { get; init; }
}