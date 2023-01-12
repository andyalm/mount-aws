using MountAnything.Routing;

namespace MountAws.Services.Ecs;

public class EcsRoutes : IServiceRoutes
{
    public void AddServiceRoutes(Route route)
    {
        route.MapLiteral<ECSRootHandler>("ecs", ecs =>
        {
            ecs.MapLiteral<ClustersHandler>("clusters", clusters =>
            {
                clusters.Map<ClusterHandler, CurrentCluster>(cluster =>
                {
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
            ecs.MapLiteral<TaskDefinitionsHandler>("task-definitions", taskFamilies =>
            {
                taskFamilies.Map<TaskFamilyHandler>(taskFamily =>
                {
                    taskFamily.Map<TaskDefinitionHandler>();
                });
            });
        });
    }
}