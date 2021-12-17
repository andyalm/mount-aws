using MountAnything;
using MountAws.Services.EC2;

namespace MountAws;

public static class LinkGeneratorExtensions
{
    public static EC2InstanceItem EC2Instance(this LinkGenerator linkGenerator, Amazon.EC2.Model.Instance instance)
    {
        var ec2ServicePath = linkGenerator.EC2ServicePath();
        var parentPath = ItemPath.Combine(ec2ServicePath, "instances");

        return new EC2InstanceItem(parentPath, instance);
    }

    private static string EC2ServicePath(this LinkGenerator linkGenerator)
    {
        return linkGenerator.ConstructPath(2, "ec2");
    }
}