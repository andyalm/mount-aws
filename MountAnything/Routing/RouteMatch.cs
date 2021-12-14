using System.Collections.Immutable;

namespace MountAnything.Routing;

public record RouteMatch(string Path, Type HandlerType)
{
    public ImmutableDictionary<string, string> Values { get; init; } = ImmutableDictionary<string, string>.Empty;
}