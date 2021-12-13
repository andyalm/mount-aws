using System.Collections.Immutable;
using Autofac;

namespace MountAnything.Routing;

public record RouteMatch(string Path, Type HandlerType)
{
    public ImmutableDictionary<string, string> Values { get; init; } = ImmutableDictionary<string, string>.Empty;
    public Action<ContainerBuilder> ServiceRegistrations { get; init; } = _ => { };
}