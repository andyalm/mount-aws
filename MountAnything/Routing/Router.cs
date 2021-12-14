using Autofac;
using Autofac.Features.ResolveAnything;

namespace MountAnything.Routing;

public class Router : IRouter
{
    private readonly List<Route> _routes = new();
    private readonly Type? _rootHandlerType;
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

    public void RouteToHandler(string path, IPathHandlerContext context, Action<IPathHandler> action)
    {
        RouteToHandler<object?>(path, context, handler =>
        {
            action(handler);

            return null;
        });
    }

    public TReturn RouteToHandler<TReturn>(string path, IPathHandlerContext context, Func<IPathHandler,TReturn> action)
    {
        var resolver = GetResolver(path);
        using var lifetimeScope = _rootContainer.Value.BeginLifetimeScope(resolver.ServiceRegistrations);
        var handler = (IPathHandler)lifetimeScope.Resolve(resolver.HandlerType,
            new NamedParameter("path", path),
            new TypedParameter(typeof(IPathHandlerContext), context));

        return action(handler);
    }

    private HandlerResolver GetResolver(string path)
    {
        if (string.IsNullOrEmpty(path) && _rootHandlerType != null)
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