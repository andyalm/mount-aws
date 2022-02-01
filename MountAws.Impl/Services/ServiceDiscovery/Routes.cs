using MountAnything.Routing;

namespace MountAws.Services.ServiceDiscovery;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<ServiceDiscoveryRootHandler>("service-discovery", serviceDiscovery =>
        {
            serviceDiscovery.MapLiteral<NamespacesHandler>("namespaces", namespaces =>
            {
                namespaces.Map<NamespaceHandler>(@namespace =>
                {
                    @namespace.Map<ServiceHandler>(service =>
                    {
                        service.Map<InstanceHandler>();
                    });
                });
            });
        });
    }
}