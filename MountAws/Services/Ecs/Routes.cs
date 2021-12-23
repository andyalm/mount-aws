using Autofac;
using MountAnything.Routing;

namespace MountAws.Services.Ecs;

public static class Routes
{
    public static void MapEcs(this Route route)
    {
        route.MapLiteral<ECSRootHandler>("ecs", ecs =>
        {
            ecs.MapLiteral<ClustersHandler>("clusters", clusters =>
            {
                clusters.Map<ClusterHandler>("CurrentCluster", cluster =>
                {
                    cluster.RegisterServices((match, builder) =>
                    {
                        builder.RegisterInstance(new CurrentCluster(match.Values["CurrentCluster"]));
                    });
                    cluster.MapLiteral<ContainerInstancesHandler>("container-instances", containerInstances =>
                    {
                        containerInstances.Map<ContainerInstanceHandler>(containerInstance =>
                        {
                            containerInstance.Map<TaskHandler>();
                        });
                    });
                    cluster.MapLiteral<ServicesHandler>("services", services =>
                    {
                        services.Map<ServiceHandler>(service =>
                        {
                            service.Map<TaskHandler>();
                        });
                    });
                });
            });
        });
    }
}