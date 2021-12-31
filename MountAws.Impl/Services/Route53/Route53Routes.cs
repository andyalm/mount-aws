using MountAnything.Routing;

namespace MountAws.Services.Route53;

public class Route53Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route route)
    {
        route.MapLiteral<Route53RootHandler>("route53", route53 =>
        {
            route53.MapLiteral<HostedZonesHandler>("hosted-zones", hostedZones =>
            {
                hostedZones.Map<HostedZoneHandler>(hostedZone =>
                {
                    hostedZone.Map<ResourceRecordHandler>();
                });
            });
        });
    }
}