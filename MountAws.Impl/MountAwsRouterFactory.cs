using MountAnything.Routing;
using MountAws.Host.Abstractions;
using MountAws.Services.Core;

namespace MountAws;

public class MountAwsRouterFactory : IRouterFactory
{
    public Router CreateRouter()
    {
        var router = Router.Create<ProfilesHandler>();
        router.RegisterServices(builder =>
        {
            typeof(CoreServiceRegistrar).Assembly
                .GetTypes()
                .Where(t => typeof(IServiceRegistrar).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IServiceRegistrar>()
                .ForEach(registrar => registrar.Register(builder));
        });
        router.Map<ProfileHandler, CurrentProfile>(profile =>
        {
            profile.Map<RegionHandler, CurrentRegion>( region =>
            {
                typeof(IServiceRoutes).Assembly
                    .GetTypes()
                    .Where(t => typeof(IServiceRoutes).IsAssignableFrom(t) && !t.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IServiceRoutes>()
                    .ForEach(serviceRouter => serviceRouter.AddServiceRoutes(region));
            });
        });

        return router;
    }
}