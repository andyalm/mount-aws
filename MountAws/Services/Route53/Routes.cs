using MountAnything.Routing;

namespace MountAws.Services.Route53;

public static class Routes
{
    public static void MapRoute53(this Route route)
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