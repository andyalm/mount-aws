using Amazon.ECS.Model;
using Task = Amazon.ECS.Model.Task;

namespace MountAws.Services.Ecs;

public static class EcsModelExtensions
{
    public static string ClusterName(this Service service)
    {
        return ClusterNameFromArn(service.ClusterArn);
    }

    public static string ClusterName(this ContainerInstance containerInstance)
    {
        return containerInstance.ContainerInstanceArn.Split(":").Last().Split("/")[1];
    }

    public static string Id(this ContainerInstance containerInstance)
    {
        return containerInstance.ContainerInstanceArn.Split("/").Last();
    }

    public static string Id(this Task task)
    {
        return task.TaskArn.Split("/").Last();
    }

    public static string ClusterName(this Task task)
    {
        return ClusterNameFromArn(task.ClusterArn);
    }

    private static string ClusterNameFromArn(string clusterArn)
    {
        return clusterArn.Split(":").Last().Split("/")[1];
    }
    
}