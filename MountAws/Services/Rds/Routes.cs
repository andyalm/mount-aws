using MountAnything.Routing;

namespace MountAws.Services.Rds;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<RootHandler>("rds", rds =>
        {
            rds.MapLiteral<ClustersHandler>("clusters", clusters =>
            {
                clusters.Map<ClusterHandler>(cluster =>
                {
                    cluster.Map<ClusterInstanceHandler>();
                });
            });
            rds.MapLiteral<InstancesHandler>("instances", instances =>
            {
                instances.Map<InstanceHandler>();
            });
        });
    }
}