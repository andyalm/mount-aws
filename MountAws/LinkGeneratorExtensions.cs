using Amazon.EC2.Model;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws;

public static class LinkGeneratorExtensions
{
    private static ItemPath EcsServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ecs");
    }

    public static ItemPath TaskDefinition(this LinkGenerator linkGenerator, string taskDefinitionArn)
    {
        var ecsServicePath = linkGenerator.EcsServicePath();
        var taskDefinition = taskDefinitionArn.Split("/").Last().Replace(":", "/");

        return ecsServicePath.Combine("task-definitions", taskDefinition);
    }

    public static ItemPath EcsCluster(this LinkGenerator linkGenerator, string clusterName)
    {
        return linkGenerator.EcsServicePath().Combine("clusters", clusterName);
    }

    public static ItemPath EcsService(this LinkGenerator linkGenerator, string clusterName, string serviceName)
    {
        return linkGenerator.EcsCluster(clusterName).Combine("services", serviceName);
    }
}