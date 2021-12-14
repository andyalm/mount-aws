using Autofac;

namespace MountAnything.Routing;

public record HandlerResolver(Type HandlerType, Action<ContainerBuilder> ServiceRegistrations);