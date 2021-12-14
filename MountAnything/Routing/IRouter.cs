namespace MountAnything.Routing;

public interface IRouter
{
    void MapRegex<T>(string pattern, Action<Route>? createChildRoutes = null) where T : IPathHandler;
}