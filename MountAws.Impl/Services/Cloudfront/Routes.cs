using MountAnything.Routing;

namespace MountAws.Services.Cloudfront;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<CloudfrontRootHandler>("cloudfront", cloudfront =>
        {
            cloudfront.MapLiteral<DistributionsHandler>("distributions", distributions =>
            {
                distributions.Map<DistributionHandler>();
            });
        });
    }
}