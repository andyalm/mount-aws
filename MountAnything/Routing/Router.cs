using Autofac;
using Autofac.Features.ResolveAnything;

namespace MountAnything.Routing;

public class Router : IRoutable
{
    private readonly List<Route> _routes = new();
    private readonly Type _rootHandlerType;
    private readonly Lazy<IContainer> _rootContainer;
    private Action<ContainerBuilder> _serviceRegistrations = _ => {};

    public static Router Create<T>() where T : IPathHandler
    {
        return new Router(typeof(T));
    }

    private Router(Type rootHandlerType)
    {
        _rootHandlerType = rootHandlerType;
        _rootContainer = new Lazy<IContainer>(CreateRootContainer);
    }

    public void MapRegex<T>(string pattern, Action<Route>? createChildRoutes = null) where T : IPathHandler
    {
        var route = new Route(pattern, typeof(T));
        createChildRoutes?.Invoke(route);
        _routes.Add(route);
    }

    public void RegisterServices(Action<ContainerBuilder> serviceRegistration)
    {
        _serviceRegistrations += serviceRegistration;
    }

    public (IPathHandler Handler, ILifetimeScope Container) RouteToHandler(ItemPath path, IPathHandlerContext context)
    {
        var resolver = GetResolver(path);
        var lifetimeScope = _rootContainer.Value.BeginLifetimeScope(resolver.ServiceRegistrations);
        var handler = (IPathHandler)lifetimeScope.Resolve(resolver.HandlerType,
            new TypedParameter(typeof(ItemPath), path),
            new TypedParameter(typeof(IPathHandlerContext), context));

        return (handler, lifetimeScope);
    }
    
    public HandlerResolver GetResolver(ItemPath path)
    {
        if (path.IsRoot)
        {
            return new HandlerResolver(_rootHandlerType, _serviceRegistrations);
        }
        
        foreach (var route in _routes)
        {
            if (route.TryGetResolver(path, out var match))
            {
                return match;
            }
        }

        throw new RoutingException($"No route matches path '{path}'");
    }
    
    private IContainer CreateRootContainer()
    {
        var builder = new ContainerBuilder();
        builder.RegisterInstance(this);
        _serviceRegistrations.Invoke(builder);
        builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

        return builder.Build();
    }
}