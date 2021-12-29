using MountAnything.Routing;

namespace MountAws.Host.Abstractions;

public interface IRouterFactory
{
    Router CreateRouter();
}
