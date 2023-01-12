using System.Management.Automation;
using MountAnything;
using MountAnything.Routing;
using MountAws.Services.Core;

namespace MountAws;

public class MountAwsProvider : IMountAnythingProvider
{
    public Router CreateRouter()
    {
        var router = Router.Create<ProfilesHandler>();
        router.ConfigureContainer(builder =>
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

    public IEnumerable<PSDriveInfo> GetDefaultDrives(ProviderInfo providerInfo)
    {
        yield return new PSDriveInfo("aws", providerInfo,
            root: "",
            description: "Allows you to navigate aws services as a virtual filesystem",
            credential: null);
    }
}