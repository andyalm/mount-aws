using Amazon.EC2.Model;
using MountAnything;
using MountAws.Services.Ec2;

namespace MountAws;

public static class LinkGeneratorExtensions
{
    public static InstanceItem EC2Instance(this LinkGenerator linkGenerator, Instance instance)
    {
        var ec2ServicePath = linkGenerator.EC2ServicePath();
        var parentPath = ec2ServicePath.Combine("instances");

        return new InstanceItem(parentPath, instance);
    }

    public static ItemPath TaskDefinition(this LinkGenerator linkGenerator, string taskDefinitionArn)
    {
        var ecsServicePath = linkGenerator.EcsServicePath();
        var taskDefinition = taskDefinitionArn.Split("/").Last().Replace(":", "/");

        return ecsServicePath.Combine("task-families", taskDefinition);
    }

    private static ItemPath EC2ServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ec2");
    }
    
    private static ItemPath EcsServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ecs");
    }
}