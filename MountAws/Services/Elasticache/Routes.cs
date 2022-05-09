using MountAnything.Routing;

namespace MountAws.Services.Elasticache;

public class Routes : IServiceRoutes
{
    public void AddServiceRoutes(Route regionRoute)
    {
        regionRoute.MapLiteral<RootHandler>("elasticache", elasticache =>
        {
            elasticache.MapLiteral<ClustersHandler>("clusters", clusters =>
            {
                clusters.Map<ClusterHandler>(cluster =>
                {
                    cluster.Map<CacheNodeHandler>();
                });
            });
            elasticache.MapLiteral<ReplicationGroupsHandler>("replication-groups", replicationGroups =>
            {
                replicationGroups.Map<ReplicationGroupHandler>(replicationGroup =>
                {
                    replicationGroup.Map<ClusterHandler>(cluster =>
                    {
                        cluster.Map<CacheNodeHandler>();
                    });
                });
            });
        });
    }
}