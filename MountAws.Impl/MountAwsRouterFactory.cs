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
            var registrars = typeof(CoreServiceRegistrar).Assembly
                .GetTypes()
                .Where(t => typeof(IServiceRegistrar).IsAssignableFrom(t) && !t.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IServiceRegistrar>();
            foreach (var registrar in registrars)
            {
                registrar.Register(builder);
            }
        });
        router.Map<ProfileHandler, CurrentProfile>(profile =>
        {
            profile.Map<RegionHandler, CurrentRegion>( region =>
            {
                var serviceRoutes = typeof(IServiceRoutes).Assembly
                    .GetTypes()
                    .Where(t => typeof(IServiceRoutes).IsAssignableFrom(t) && !t.IsAbstract)
                    .Select(Activator.CreateInstance)
                    .Cast<IServiceRoutes>();
                foreach (var serviceRouter in serviceRoutes)
                {
                    serviceRouter.AddServiceRoutes(region);
                }
            });
        });

        return router;
    }
}