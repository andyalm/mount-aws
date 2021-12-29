using Autofac;

namespace MountAnything.Routing;

public interface IRoutable
{
    void MapRegex<T>(string pattern, Action<Route>? createChildRoutes = null) where T : IPathHandler;
}