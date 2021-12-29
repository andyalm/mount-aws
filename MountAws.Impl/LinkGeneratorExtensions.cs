using Amazon.EC2.Model;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws;

public static class LinkGeneratorExtensions
{
    public static InstanceItem EC2Instance(this LinkGenerator linkGenerator, Instance instance)
    {
        var ec2ServicePath = linkGenerator.EC2ServicePath();
        var parentPath = ItemPath.Combine(ec2ServicePath, "instances");

        return new InstanceItem(parentPath, instance);
    }

    public static string TaskDefinition(this LinkGenerator linkGenerator, string taskDefinitionArn)
    {
        var ecsServicePath = linkGenerator.EcsServicePath();
        var taskDefinition = taskDefinitionArn.Split("/").Last().Replace(":", "/");

        return ItemPath.Combine(ecsServicePath, "task-families", taskDefinition);
    }

    private static string EC2ServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ec2");
    }
    
    private static string EcsServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ecs");
    }
}